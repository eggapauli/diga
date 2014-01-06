﻿using Svc = Diga.Domain.Service;
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
        private static readonly string sampleProblemTaskKey = "SampleTSPProblem";

        static void Main(string[] args)
        {
            bool isValidInput;
            var digaCallback = new DigaCallback();

            using (var channelFactory = new DuplexChannelFactory<Svc.Contracts.IDigaService>(new InstanceContext(digaCallback), "DigaService_DualHttpEndpoint")) {
                var digaService = channelFactory.CreateChannel();

                do {
                    Console.WriteLine("What would you like to do?");
                    Console.WriteLine("0 ... Exit");
                    Console.WriteLine("1 ... Add sample problem");
                    Console.WriteLine("2 ... Participate in solving a sample problem");
                    Console.WriteLine("3 ... Retrieve the best solution of a sample problem");
                    Console.WriteLine("4 ... Clear results");

                    int choice;
                    isValidInput = int.TryParse(Console.ReadLine(), out choice);
                    if (isValidInput) {
                        switch (choice) {
                            case 1:
                                AddSampleProblem(digaService, sampleProblemTaskKey);
                                break;
                            case 2:
                                SolveSampleProblemAsync(digaService, digaCallback, sampleProblemTaskKey).Wait();
                                break;
                            case 3:
                                ShowSampleProblemSolutionAsync(digaService, sampleProblemTaskKey).Wait();
                                break;
                            case 4:
                                ClearSampleProblemResultAsync(digaService).Wait();
                                break;
                            default:
                                isValidInput = false;
                                break;
                        }
                    }
                } while (isValidInput);
            }
        }

        private static void AddSampleProblem(IDigaService digaService, string taskKey)
        {
            var parameters = new Domain.Parameters.IslandGAParameters(
                crossover: new Domain.Crossovers.MaximalPreservativeCrossover(),
                elites: 1,
                emigrantsSelector: new Domain.Selectors.BestSelector(),
                immigrationReplacer: new Domain.ImmigrationReplacers.WorstReplacer(),
                maximumMigrations: 3,
                migrationInterval: 50,
                migrationRate: 2,
                migrator: new Domain.Migrators.UnidirectionalRingMigrator(),
                mutationProbability: 0.1,
                mutator: new Domain.Mutators.InversionManipulator(),
                populationSize: 500,
                seed: 0,
                selector: new Domain.Selectors.BestSelector(),
                setSeedRandomly: true);
            var task = new Domain.OptimizationTask(new Domain.Problems.TSP(), new Domain.Algorithms.IslandGA(parameters));

            var serviceTask = (Svc.DataContracts.OptimizationTask)Svc.Converter.ConvertFromDomainToService(task);
            try {
                digaService.AddOptimizationTask(taskKey, serviceTask);
                Console.WriteLine("The task has been added.");
            }
            catch (FaultException<TaskNotAddedFault>) {
                Console.Error.WriteLine("The task couldn't be added.");
            }
        }

        private static async Task SolveSampleProblemAsync(Svc.Contracts.IDigaService digaService, IDigaCallback digaCallback, string taskKey)
        {
            try {
                var taskData = digaService.GetOptimizationTask(taskKey);
                Domain.OptimizationTask task = (Domain.OptimizationTask)Svc.Converter.ConvertFromServiceToDomain(taskData);

                Console.WriteLine("Start calculating {0}.", taskKey);
                int iterationNumber = task.Algorithm.Migrations + 1;
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

                await digaService.SetResultAsync(taskKey, (AbstractSolution)Svc.Converter.ConvertFromDomainToService(task.Algorithm.BestSolution));

                Console.WriteLine("Finished calculating {0}.", taskKey);
            }
            catch (FaultException<TaskNotFoundFault>) {
                Console.Error.WriteLine("The task couldn't be found.");
            }
        }

        private static async Task ShowSampleProblemSolutionAsync(IDigaService digaService, string taskKey)
        {
            try {
                var result = await digaService.GetResultAsync(taskKey);
                Console.WriteLine("Best Quality: {0:F2}", result.BestQuality);
                Console.WriteLine("Number of Workers: {0}", result.NumberOfWorkers);
                Console.WriteLine("Run Duration: {0}", result.RunDuration);
            }
            catch (FaultException<TaskNotFoundFault>) {
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
