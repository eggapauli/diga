using Diga.Domain.Contracts;
using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Parameters;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;
using Diga.Domain.Service.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Diga.Domain.Service
{
    public class DigaServiceLogic : IDigaService, IDigaStatusService
    {
        private static readonly string serviceToWorkerQueueName = "work";
        private QueueClient serviceToWorkerQueue;

        private static readonly string workerToServiceQueueName = "results";
        private QueueClient workerToServiceQueue;

        private readonly object migrationLocker = new object();

        public DigaServiceLogic()
        {
            InitQueues();
        }

        private void InitQueues()
        {
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (namespaceManager.QueueExists(serviceToWorkerQueueName)) {
                namespaceManager.DeleteQueue(serviceToWorkerQueueName);
            }
            namespaceManager.CreateQueue(serviceToWorkerQueueName);
            serviceToWorkerQueue = QueueClient.CreateFromConnectionString(connectionString, serviceToWorkerQueueName);

            if (namespaceManager.QueueExists(workerToServiceQueueName)) {
                namespaceManager.DeleteQueue(workerToServiceQueueName);
            }
            namespaceManager.CreateQueue(workerToServiceQueueName);
            workerToServiceQueue = QueueClient.CreateFromConnectionString(connectionString, workerToServiceQueueName);

            workerToServiceQueue.OnMessageAsync(async message => {
                string type = (string)message.Properties["Type"];
                string taskKey = (string)message.Properties["TaskKey"];

                if (type == "Migration") {
                    var solutions = message.GetBody<IEnumerable<AbstractSolution>>();
                    var worker = StateManager.Instance
                        .GetWorkers(taskKey)
                        .OfType<ServiceBusQueueWorker>()
                        .Single(w => w.Id == (Guid)message.Properties["WorkerId"]);

                    Migrate(taskKey, solutions, worker);
                }
                else if (type == "Result") {
                    try {
                        var serializer = new DataContractSerializer(typeof(AbstractSolution));
                        var solution = message.GetBody<AbstractSolution>(serializer);
                        await SetResultAsync(taskKey, solution);
                    }
                    catch (Exception e) {

                    }
                }
                else {
                    throw new NotSupportedException("Invalid message type " + type);
                }
            }, new OnMessageOptions { AutoComplete = true });
        }

        public void AddOptimizationTask(DataContracts.OptimizationTask task)
        {
            if (!StateManager.Instance.AddTask(task)) {
                throw new FaultException<TaskNotAddedFault>(new TaskNotAddedFault());
            }

            StateManager.Instance.AddWorker(task.TaskKey, new ServiceBusQueueWorker(serviceToWorkerQueue));
        }

        public void ApplyForCalculatingOptimizationTask(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null) {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }
            else if (task.EndTime.HasValue) {
                throw new FaultException<TaskFinishedFault>(new TaskFinishedFault());
            }

            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();
            StateManager.Instance.AddWorker(taskKey, new WcfWorker(channel));
        }

        public void StartOptimizationTask(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null) {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }
            else if (task.EndTime.HasValue) {
                throw new FaultException<TaskFinishedFault>(new TaskFinishedFault());
            }

            task.StartTime = task.StartTime ?? DateTime.Now;

            foreach (var worker in StateManager.Instance.GetWorkers(taskKey)) {
                worker.StartWork(task);
            }
        }

        public void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions)
        {
            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();
            var worker = StateManager.Instance.GetWorkers(taskKey).OfType<WcfWorker>().Single(w => w.CallbackChannel == channel);

            Migrate(taskKey, solutions, worker);
        }

        private void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions, IRemoteWorker worker)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null) {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            if (task.Algorithm.Migrations >= task.Algorithm.Parameters.MaximumMigrations - 1) {
                task.EndTime = task.EndTime ?? DateTime.Now;
                worker.Migrate(null);
            }
            else {
                var workers = StateManager.Instance.GetWorkers(taskKey);
                StateManager.Instance.AddMigration(taskKey, worker, solutions);

                lock (migrationLocker) {
                    var migrations = StateManager.Instance.GetMigrations(taskKey);
                    if (workers.Count == migrations.Count) {
                        StateManager.Instance.ResetMigrations(taskKey);
                        task.Algorithm.Migrations++;

                        var migrator = (IMigrator)Converter.ConvertFromServiceToDomain(((IslandGAParameters)task.Algorithm.Parameters).Migrator);
                        var map = migrator.GetMigrationMap(workers.Count);
                        foreach (var pair in map.Select((item, index) => new { Source = item, Target = index })) {
                            var source = workers[pair.Source];
                            var sourceEmmigrants = migrations[source];
                            var target = workers[pair.Target];
                            target.Migrate(solutions);
                        }
                    }
                }
            }
        }

        public async Task SetResultAsync(string taskKey, AbstractSolution bestSolution)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null) {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            await StateManager.Instance.UpdateResultAsync(taskKey, bestSolution);
        }

        public async Task<DataContracts.Result> GetResultAsync(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null) {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            return await StateManager.Instance.GetResultAsync(taskKey);
        }

        public async Task ClearResultsAsync()
        {
            await StateManager.Instance.ClearResultsAsync();

            InitQueues();
        }

        public DataContracts.OptimizationTask GetOptimizationTask(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null) {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }
            return task;
        }

        public IEnumerable<string> GetAllOptimizationTaskKeys()
        {
            return StateManager.Instance.GetAllTaskKeys();
        }
    }
}
