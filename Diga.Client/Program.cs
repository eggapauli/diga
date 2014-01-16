using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using Svc = Diga.Domain.Service;

namespace Diga.Client
{
    class Program
    {
        private static readonly string sampleProblemTaskKey = "SampleTSPProblem";

        static void Main(string[] args)
        {
            var digaCallback = new DigaCallback();

            string argument = args.SkipWhile(arg => arg != "-choice").Skip(1).FirstOrDefault();
            bool exitAfterAction = args.Any(arg => arg == "-exitAfterAction");

            using (var channelFactory = new DuplexChannelFactory<Svc.Contracts.IDigaService>(new InstanceContext(digaCallback), "DigaService_DualHttpEndpoint"))
            {
                var digaService = channelFactory.CreateChannel();

                var actions = new List<KeyValuePair<string, Action>> {
                    new KeyValuePair<string, Action>("Add sample problem", () => AddSampleProblem(digaService, sampleProblemTaskKey)),
                    new KeyValuePair<string, Action>("Participate in solving a sample problem", () => SolveSampleProblemAsync(digaService, digaCallback, sampleProblemTaskKey).Wait()),
                    new KeyValuePair<string, Action>("Start clients to solve a sample problem", () => StartClients()),
                    new KeyValuePair<string, Action>("Retrieve the best solution of a sample problem", () => ShowSampleProblemSolutionAsync(digaService, sampleProblemTaskKey).Wait()),
                    new KeyValuePair<string, Action>("Clear results", () => ClearSampleProblemResultAsync(digaService).Wait()),
                };

                bool isValidInput;
                do
                {
                    int choice;
                    if (argument != null)
                    {
                        isValidInput = int.TryParse(argument, out choice);
                        argument = null;
                    }
                    else
                    {
                        Console.WriteLine("What would you like to do?");
                        Console.WriteLine("0 ... Exit");
                        foreach (var action in actions.Select((a, i) => new { Action = a, Index = i + 1 }))
                        {
                            Console.WriteLine("{0} ... {1}", action.Index, action.Action.Key);
                        }

                        isValidInput = int.TryParse(Console.ReadLine(), out choice);
                    }

                    isValidInput = isValidInput && choice > 0 && choice <= actions.Count;

                    var selectedAction = isValidInput ? actions[choice - 1].Value : null;
                    if (selectedAction != null)
                    {
                        selectedAction();
                    }
                } while (isValidInput && !exitAfterAction);
            }
        }

        private static void AddSampleProblem(IDigaService digaService, string taskKey)
        {
            var parameters = new Domain.Parameters.IslandGAParameters(
                crossover: new Domain.Crossovers.MaximalPreservativeCrossover(),
                elites: 1,
                emigrantsSelector: new Domain.Selectors.BestSelector(),
                evaluator: new Domain.Evaluators.TSPSolutionEvaluator(),
                immigrationReplacer: new Domain.ImmigrationReplacers.WorstReplacer(),
                maximumMigrations: 20,
                migrationInterval: 50,
                migrationRate: 0.25,
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

            var serviceTask = (Svc.DataContracts.OptimizationTask)Svc.Converter.ConvertFromDomainToService(task);
            try
            {
                digaService.AddOptimizationTask(taskKey, serviceTask);
                Console.WriteLine("The task has been added.");
            }
            catch (FaultException<TaskNotAddedFault>)
            {
                Console.Error.WriteLine("The task couldn't be added.");
            }
        }

        private static async Task SolveSampleProblemAsync(Svc.Contracts.IDigaService digaService, IDigaCallback digaCallback, string taskKey)
        {
            try
            {
                var taskData = digaService.ApplyForCalculatingOptimizationTask(taskKey);
                var task = (Domain.OptimizationTask)Svc.Converter.ConvertFromServiceToDomain(taskData);

                Console.WriteLine("Started initializing {0}.", taskKey);
                await task.Algorithm.InitializeAsync(task.Problem);
                Console.WriteLine("Finished initializing {0}.", taskKey);

                Console.WriteLine("Start calculating {0}.", taskKey);
                int iterationNumber = 0;
                bool finished = false;
                do
                {
                    iterationNumber++;
                    Console.WriteLine("Starting iteration {0}.", iterationNumber);
                    await task.Algorithm.CalculateAsync(task.Problem);
                    var emigrants = task.Algorithm.ReleaseEmigrants(task.Problem);
                    var solutionData = emigrants.Select(solution => (AbstractSolution)Svc.Converter.ConvertFromDomainToService(solution));

                    var waitForMigrationTask = digaCallback.WaitForMigrationAsync();
                    digaService.Migrate(taskKey, solutionData);
                    var immigrants = await waitForMigrationTask;
                    if (immigrants != null)
                    {
                        task.Algorithm.AddImmigrants(immigrants.Select(solution => (Domain.Contracts.ISolution)Svc.Converter.ConvertFromServiceToDomain(solution)), task.Problem);
                    }
                    finished = immigrants == null;
                } while (!finished);

                await digaService.SetResultAsync(taskKey, (AbstractSolution)Svc.Converter.ConvertFromDomainToService(task.Algorithm.BestSolution));

                Console.WriteLine("Finished calculating {0}.", taskKey);
            }
            catch (FaultException<TaskNotFoundFault>)
            {
                Console.Error.WriteLine("The task couldn't be found.");
            }
            catch (FaultException<TaskFinishedFault>)
            {
                Console.Error.WriteLine("The task has already been finished.");
            }
        }

        private static void StartClients()
        {
            var processPath = Assembly.GetExecutingAssembly().Location;

            foreach (var i in Enumerable.Range(0, 5))
            {
                Process.Start(processPath, "-choice 2 -exitAfterAction");
            }
        }

        private static async Task ShowSampleProblemSolutionAsync(IDigaService digaService, string taskKey)
        {
            try
            {
                var result = await digaService.GetResultAsync(taskKey);
                Console.WriteLine("Best Quality: {0:F2}", result.BestQuality);
                Console.WriteLine("Number of Workers: {0}", result.NumberOfWorkers);
                Console.WriteLine("Run Duration: {0}", result.RunDuration);
            }
            catch (FaultException<TaskNotFoundFault>)
            {
                Console.Error.WriteLine("The task couldn't be found.");
            }
        }

        private static async Task ClearSampleProblemResultAsync(IDigaService digaService)
        {
            await digaService.ClearResultsAsync();
            Console.WriteLine("The table has been cleared.");
        }
    }
}
