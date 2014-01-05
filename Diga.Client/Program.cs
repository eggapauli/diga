using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Svc = Diga.Domain.Service;

namespace Diga.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var digaCallback = new DigaCallback();

            using (var channelFactory = new DuplexChannelFactory<Svc.Contracts.IDigaService>(new InstanceContext(digaCallback), "DigaService_DualHttpEndpoint"))
            {
                var digaService = channelFactory.CreateChannel();

                var parameters = new Domain.Parameters.IslandGAParameters(
                    crossover: new Domain.Crossovers.MaximalPreservativeCrossover(),
                    elites: 1,
                    emigrantsSelector: new Domain.Selectors.BestSelector(),
                    evaluator: new Domain.Evaluators.TSPSolutionEvaluator(),
                    immigrationReplacer: new Domain.ImmigrationReplacers.WorstReplacer(),
                    maximumMigrations: 5,
                    migrationInterval: 50,
                    migrationRate: 0.15,
                    migrator: new Domain.Migrators.UnidirectionalRingMigrator(),
                    mutationProbability: 0.05,
                    mutator: new Domain.Mutators.InversionManipulator(),
                    populationSize: 100,
                    seed: 0,
                    selector: new Domain.Selectors.BestSelector(),
                    setSeedRandomly: true,
                    solutionCreator: new Domain.SolutionCreators.RandomTSPSolutionCreator()
                  );

                var task = new Domain.OptimizationTask(new Domain.Problems.TSP("ch130"), new Domain.Algorithms.IslandGA(parameters));
                string taskKey = "SampleTSPProblem";

                var serviceTask = (Svc.DataContracts.OptimizationTask)Svc.Converter.ConvertFromDomainToService(task);
                try
                {
                    digaService.AddOptimizationTask(taskKey, serviceTask);
                }
                catch (FaultException<TaskNotAddedFault>)
                {
                    Console.Error.WriteLine("The task couldn't be added.");
                }

                CalculateAsync(digaService, digaCallback, taskKey).Wait();
            }
        }

        private static async Task CalculateAsync(Svc.Contracts.IDigaService digaService, IDigaCallback digaCallback, string taskKey)
        {
            try
            {
                var taskData = digaService.GetOptimizationTask("SampleTSPProblem");
                Domain.OptimizationTask task = (Domain.OptimizationTask)Svc.Converter.ConvertFromServiceToDomain(taskData);

                Console.WriteLine("Started initializing {0}.", taskKey);
                await task.Algorithm.InitializeAsync(task.Problem);
                Console.WriteLine("Finished initializing {0}.", taskKey);

                Console.WriteLine("Started calculating {0}.", taskKey);
                int iterationNumber = 1;
                do
                {
                    Console.WriteLine("Starting iteration {0}.", iterationNumber);
                    await task.Algorithm.CalculateAsync(task.Problem);
                    var emigrants = task.Algorithm.ReleaseEmigrants(task.Problem);
                    var solutionData = emigrants.Select(solution => (AbstractSolution)Svc.Converter.ConvertFromDomainToService(solution));

                    digaService.Migrate(taskKey, solutionData);
                    var immigrants = await digaCallback.WaitForMigrationAsync();
                    task.Algorithm.AddImmigrants(immigrants.Select(solution => (Domain.Contracts.ISolution)Svc.Converter.ConvertFromServiceToDomain(solution)), task.Problem);
                    iterationNumber++;
                } while (!digaCallback.FinishToken.IsCancellationRequested);

                digaService.SetResult(taskKey, (AbstractSolution)Svc.Converter.ConvertFromDomainToService(task.Algorithm.BestSolution));

                Console.WriteLine("Finished calculating {0}.", taskKey);
            }
            catch (FaultException<TaskNotFoundFault>)
            {
                Console.Error.WriteLine("The task couldn't be found.");
            }
        }
    }
}
