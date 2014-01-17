using DataContracts = Diga.Domain.Service.DataContracts;
using Svc = Diga.Domain.Service;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Diga.Domain.Service.DataContracts.Solutions;
using System.ServiceModel;
using Diga.Domain.Service.FaultContracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Diga.Cloud.Client
{
    public class WorkerRole : RoleEntryPoint
    {
        private static readonly string serviceToWorkerQueueName = "work";
        private static readonly string workerToServiceQueueName = "results";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient serviceToWorkerQueue;
        QueueClient workerToServiceQueue;
        ManualResetEvent completedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            serviceToWorkerQueue.OnMessage(message => {
                var task = message.GetBody<DataContracts.OptimizationTask>();
                message.Complete();
                Calculate(task, (Guid)message.Properties["WorkerId"]).Wait();
            });

            completedEvent.WaitOne();
        }

        private async Task Calculate(DataContracts.OptimizationTask taskData, Guid workerId)
        {
            try {
                var task = (Domain.OptimizationTask)Svc.Converter.ConvertFromServiceToDomain(taskData);

                Trace.WriteLine(string.Format("Started initializing {0}.", task.TaskKey));
                await task.Algorithm.InitializeAsync(task.Problem);
                Trace.WriteLine(string.Format("Finished initializing {0}.", task.TaskKey));

                Trace.WriteLine(string.Format("Start calculating {0}.", task.TaskKey));
                int iterationNumber = 0;
                bool finished = false;
                do {
                    iterationNumber++;
                    Trace.WriteLine(string.Format("Starting iteration {0}.", iterationNumber));
                    await task.Algorithm.CalculateAsync(task.Problem);
                    var emigrants = task.Algorithm.ReleaseEmigrants(task.Problem);
                    var solutionData = emigrants.Select(solution => (AbstractSolution)Svc.Converter.ConvertFromDomainToService(solution)).ToList();

                    var emmigrationMessage = new BrokeredMessage(solutionData);
                    emmigrationMessage.Properties["WorkerId"] = workerId;
                    emmigrationMessage.Properties["TaskKey"] = task.TaskKey;
                    emmigrationMessage.Properties["Type"] = "Migration";
                    await workerToServiceQueue.SendAsync(emmigrationMessage);

                    var immigrationMessage = await serviceToWorkerQueue.ReceiveAsync();
                    var immigrants = immigrationMessage.GetBody<IEnumerable<AbstractSolution>>();
                    immigrationMessage.Complete();

                    if (immigrants != null) {
                        task.Algorithm.AddImmigrants(immigrants.Select(solution => (Domain.Contracts.ISolution)Svc.Converter.ConvertFromServiceToDomain(solution)), task.Problem);
                    }
                    finished = immigrants == null;
                } while (!finished);

                var serializer = new DataContractSerializer(typeof(AbstractSolution));
                var resultMessage = new BrokeredMessage((AbstractSolution)Svc.Converter.ConvertFromDomainToService(task.Algorithm.BestSolution), serializer);
                resultMessage.Properties["WorkerId"] = workerId;
                resultMessage.Properties["TaskKey"] = task.TaskKey;
                resultMessage.Properties["Type"] = "Result";
                await workerToServiceQueue.SendAsync(resultMessage);

                Trace.WriteLine(string.Format("Finished calculating {0}.", task.TaskKey));
            }
            catch (FaultException<TaskNotFoundFault>) {
                Trace.WriteLine("The task couldn't be found.");
            }
            catch (FaultException<TaskFinishedFault>) {
                Trace.WriteLine("The task has already been finished.");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(serviceToWorkerQueueName)) {
                namespaceManager.CreateQueue(serviceToWorkerQueueName);
            }
            serviceToWorkerQueue = QueueClient.CreateFromConnectionString(connectionString, serviceToWorkerQueueName);

            if (!namespaceManager.QueueExists(workerToServiceQueueName)) {
                namespaceManager.CreateQueue(workerToServiceQueueName);
            }
            workerToServiceQueue = QueueClient.CreateFromConnectionString(connectionString, workerToServiceQueueName);

            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            serviceToWorkerQueue.Close();
            workerToServiceQueue.Close();
            completedEvent.Set();
            base.OnStop();
        }
    }
}
