using Diga.Domain.Contracts;
using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Parameters;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Diga.Domain.Service
{
    public class DigaServiceLogic : IDigaService, IDigaStatusService
    {
        private readonly object migrationLocker = new object();

        public void AddOptimizationTask(string taskKey, DataContracts.OptimizationTask task)
        {
            if (!StateManager.Instance.AddTask(taskKey, task))
            {
                throw new FaultException<TaskNotAddedFault>(new TaskNotAddedFault());
            }
        }

        public DataContracts.OptimizationTask ApplyForCalculatingOptimizationTask(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }
            else if (task.EndTime != null)
            {
                throw new FaultException<TaskFinishedFault>(new TaskFinishedFault());
            }

            task.StartTime = task.StartTime ?? DateTime.Now;

            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();
            StateManager.Instance.AddWorker(taskKey, channel);
            return task;
        }

        public void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions)
        {
            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();

            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            if (task.Algorithm.Migrations >= task.Algorithm.Parameters.MaximumMigrations - 1)
            {
                task.EndTime = task.EndTime ?? DateTime.Now;
                channel.Migrate(null);
            }
            else
            {
                var workers = StateManager.Instance.GetWorkers(taskKey);
                StateManager.Instance.AddMigration(taskKey, channel, solutions);

                lock (migrationLocker)
                {
                    var migrations = StateManager.Instance.GetMigrations(taskKey);
                    if (workers.Count == migrations.Count)
                    {
                        StateManager.Instance.ResetMigrations(taskKey);
                        task.Algorithm.Migrations++;

                        var migrator = (IMigrator)Converter.ConvertFromServiceToDomain(((IslandGAParameters)task.Algorithm.Parameters).Migrator);
                        var map = migrator.GetMigrationMap(workers.Count);
                        foreach (var pair in map.Select((item, index) => new { Source = item, Target = index }))
                        {
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
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            await StateManager.Instance.UpdateResultAsync(taskKey, bestSolution);
        }

        public async Task<DataContracts.Result> GetResultAsync(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            return await StateManager.Instance.GetResultAsync(taskKey);
        }

        public async Task ClearResultsAsync()
        {
            await StateManager.Instance.ClearResultsAsync();
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
