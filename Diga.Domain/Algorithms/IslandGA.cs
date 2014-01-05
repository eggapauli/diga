using Diga.Domain.Contracts;
using Diga.Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diga.Domain.Algorithms
{
    public class IslandGA : IAlgorithm
    {
        private ISolution[] population;

        public IParameters Parameters { get; private set; }

        public ISolution BestSolution { get; set; }

        public int Migrations { get; set; }

        public IslandGA() { }

        public IslandGA(IParameters parameters)
        {
            Parameters = parameters;
        }

        public Task InitializeAsync(IProblem problem)
        {
            return Task.Run(() =>
            {
                var @params = (IslandGAParameters)Parameters;

                population = new ISolution[@params.PopulationSize];
                for (int i = 0; i < population.Length; i++)
                {
                    // TODO remove length argument
                    population[i] = @params.SolutionCreator.Apply(@params.Random, 130);
                    @params.Evaluator.Apply(population[i], problem);
                }
            });
        }

        public Task CalculateAsync(IProblem problem)
        {
            return Task.Run(() =>
            {
                var @params = (IslandGAParameters)Parameters;
                var selected = @params.Selector.Apply(population, (population.Length - @params.Elites) * 2, problem.Maximization).ToArray();
                var offspring = new ISolution[selected.Length / 2];
                for (int i = 0; i < offspring.Length; i++)
                {
                    var mother = selected[i * 2];
                    var father = selected[i * 2 + 1];
                    offspring[i] = @params.Crossover.Apply(@params.Random, mother, father);
                    if (@params.Random.NextDouble() < @params.MutationProbability)
                        @params.Mutator.Apply(@params.Random, offspring[i]);
                    @params.Evaluator.Apply(offspring[i], problem);
                }
                var elites = @params.Selector.Apply(population, @params.Elites, problem.Maximization).ToArray();
                population = elites.Concat(offspring).ToArray();
                BestSolution = @params.Selector.Apply(population, 1, problem.Maximization).Single();
            });
        }

        public IEnumerable<ISolution> ReleaseEmigrants(IProblem problem)
        {
            var @params = (IslandGAParameters)Parameters;
            var emigrants = @params.Selector.Apply(population, (int)Math.Round(population.Length * @params.MigrationRate), problem.Maximization);
            return emigrants;
        }

        public void AddImmigrants(IEnumerable<ISolution> immigrants, IProblem problem)
        {
            var @params = (IslandGAParameters)Parameters;
            var best = @params.Selector.Apply(population, (int)Math.Round(population.Length * (1 - @params.MigrationRate)), problem.Maximization);
            population = best.Concat(immigrants).ToArray();
        }
    }
}
