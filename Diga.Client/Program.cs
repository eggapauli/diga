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
using Diga.Domain.Service.Contracts;

namespace Diga.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            DigaCallback digaCallback = new DigaCallback();

            using (var channelFactory = new DuplexChannelFactory<Svc.Contracts.IDigaService>(new InstanceContext(digaCallback), "DigaService_DualHttpEndpoint")) {
                var digaService = channelFactory.CreateChannel();

                var parameters = new Domain.Parameters.IslandGAParameters(new Domain.Crossovers.MaximalPreservativeCrossover(), 1, new Domain.Selectors.BestSelector(), new Domain.ImmigrationReplacers.WorstReplacer(), 5, 50, 2, new Domain.Migrators.UnidirectionalRingMigrator(), 0.1, new Domain.Mutators.InversionManipulator(), 500, 0, new Domain.Selectors.BestSelector(), true);
                var task = new Domain.OptimizationTask(new Domain.Problems.TSP(), new Domain.Algorithms.IslandGA(parameters));
                string taskKey = "SampleTSPProblem";

                var serviceTask = (Svc.DataContracts.OptimizationTask)Svc.Converter.ConvertFromDomainToService(task);
                try {
                    digaService.AddOptimizationTask(taskKey, serviceTask);
                }
                catch (FaultException<TaskNotAddedFault>) {
                    Console.Error.WriteLine("The task couldn't be added.");
                }

                CalculateAsync(digaService, digaCallback, taskKey).Wait();
            }
        }

        private static async Task CalculateAsync(Svc.Contracts.IDigaService digaService, IDigaCallback digaCallback, string taskKey)
        {
            try {
                var taskData = digaService.GetOptimizationTask("SampleTSPProblem");
                Domain.OptimizationTask task = (Domain.OptimizationTask)Svc.Converter.ConvertFromServiceToDomain(taskData);

                Console.WriteLine("Started calculating {0}.", taskKey);
                int iterationNumber = 1;
                do {
                    Console.WriteLine("Starting iteration {0}.", iterationNumber);
                    await task.Algorithm.CalculateAsync(task.Problem);
                    var emigrants = task.Algorithm.ReleaseEmigrants();
                    var solutionData = emigrants.Select(solution => (AbstractSolution)Svc.Converter.ConvertFromDomainToService(solution));

                    digaService.Migrate(taskKey, solutionData);
                    var immigrants = await digaCallback.WaitForMigrationAsync();
                    task.Algorithm.AddImmigrants(immigrants.Select(solution => (Domain.Contracts.ISolution)Svc.Converter.ConvertFromServiceToDomain(solution)));
                    iterationNumber++;
                } while (!digaCallback.FinishToken.IsCancellationRequested);

                digaService.SetResult(taskKey, (AbstractSolution)Svc.Converter.ConvertFromDomainToService(task.Algorithm.BestSolution));

                Console.WriteLine("Finished calculating {0}.", taskKey);
            }
            catch (FaultException<TaskNotFoundFault>) {
                Console.Error.WriteLine("The task couldn't be found.");
            }
        }
    }
}
