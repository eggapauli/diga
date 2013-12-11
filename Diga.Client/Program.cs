using Svc = Diga.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Diga.Domain.Service.FaultContracts;
using System.Threading;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.DataContracts;

namespace Diga.Client
{
    class Program
    {
        private static DigaCallback callback;
        private static TaskCompletionSource<IterationData> tcs;

        static void Main(string[] args)
        {
            // TODO search for better solution with tcs
            callback = new DigaCallback();
            callback.OnMigrate += (key, iterationData) => tcs.SetResult(iterationData);

            using (var channelFactory = new DuplexChannelFactory<Svc.Contracts.IDigaService>(new InstanceContext(callback), "DigaService_DualHttpEndpoint")) {
                var digaService = channelFactory.CreateChannel();

                var parameters = new Domain.Parameters.IslandGAParameters(new Domain.Crossovers.MaximalPreservativeCrossover(), 1, new Domain.Selectors.BestSelector(), new Domain.ImmigrationReplacers.WorstReplacer(), 100, 5, 2, 0.1, new Domain.Mutators.InversionManipulator(), 500, 0, new Domain.Selectors.BestSelector(), true);
                var task = new Domain.OptimizationTask(new Domain.Problems.TSP(), new Domain.Algorithms.IslandGA(parameters));
                string taskKey = "SampleTSPProblem";

                var serviceTask = (Svc.DataContracts.OptimizationTask)Svc.Converter.ConvertFromDomainToService(task);
                try {
                    digaService.AddOptimizationTask(taskKey, serviceTask);
                }
                catch (FaultException<TaskNotAddedFault>) {
                    Console.Error.WriteLine("The task couldn't be added.");
                }

                Calculate(digaService, taskKey).Wait();
            }
        }

        private static async Task Calculate(Svc.Contracts.IDigaService digaService, string taskKey)
        {
            try {
                var taskData = digaService.GetOptimizationTask("SampleTSPProblem");
                Domain.OptimizationTask task = (Domain.OptimizationTask)Svc.Converter.ConvertFromServiceToDomain(taskData);

                IterationData iterationData;
                int iterationNumber = 1;
                do {
                    Console.WriteLine("Starting iteration {0}.", iterationNumber);
                    await task.Algorithm.CalculateAsync(task.Problem);
                    var solutions = task.Algorithm.ReleaseSolutionsForMigration();
                    var solutionData = solutions.Select(s => (AbstractSolution)Svc.Converter.ConvertFromDomainToService(solutions));

                    tcs = new TaskCompletionSource<IterationData>();
                    digaService.Migrate(taskKey, solutionData);
                    iterationData = await tcs.Task;
                    iterationNumber++;
                } while (iterationData.RemainingIterations > 0);

                Console.WriteLine("Finished calculating {0}.", taskKey);
            }
            catch (FaultException<TaskNotFoundFault>) {
                Console.Error.WriteLine("The task couldn't be found.");
            }
        }
    }
}
