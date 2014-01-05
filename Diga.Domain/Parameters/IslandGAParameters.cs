using Diga.Domain.Contracts;
using System;

namespace Diga.Domain.Parameters
{
    public class IslandGAParameters : IParameters
    {
        public ICrossover Crossover { get; set; }

        public int Elites { get; set; }

        public ISelector EmigrantsSelector { get; set; }

        public IEvaluator Evaluator { get; set; }

        public IImmigrationReplacer ImmigrationReplacer { get; set; }

        public int MaximumMigrations { get; set; }

        public int MigrationInterval { get; set; }

        public double MigrationRate { get; set; }

        public IMigrator Migrator { get; set; }

        public double MutationProbability { get; set; }

        public IMutator Mutator { get; set; }

        public int PopulationSize { get; set; }

        public Random Random { get; set; }

        public int Seed { get; set; }

        public ISelector Selector { get; set; }

        public bool SetSeedRandomly { get; set; }

        public ISolutionCreator SolutionCreator { get; set; }

        public IslandGAParameters()
        {
        }

        public IslandGAParameters(
            ICrossover crossover,
            int elites,
            ISelector emigrantsSelector,
            IEvaluator evaluator,
            IImmigrationReplacer immigrationReplacer,
            int maximumMigrations,
            int migrationInterval,
            double migrationRate,
            IMigrator migrator,
            double mutationProbability,
            IMutator mutator,
            int populationSize,
            int seed,
            ISelector selector,
            bool setSeedRandomly,
            ISolutionCreator solutionCreator)
        {
            Crossover = crossover;
            Elites = elites;
            EmigrantsSelector = emigrantsSelector;
            Evaluator = evaluator;
            ImmigrationReplacer = immigrationReplacer;
            MaximumMigrations = maximumMigrations;
            MigrationInterval = migrationInterval;
            MigrationRate = migrationRate;
            Migrator = migrator;
            MutationProbability = mutationProbability;
            Mutator = mutator;
            PopulationSize = populationSize;
            Random = setSeedRandomly ? new Random() : new Random(Seed);
            Seed = seed;
            Selector = selector;
            SetSeedRandomly = setSeedRandomly;
            SolutionCreator = solutionCreator;
        }
    }
}
