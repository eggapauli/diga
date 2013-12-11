using Diga.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Parameters
{
    public class IslandGAParameters : IParameters
    {
        public ICrossover Crossover { get; set; }

        public int Elites { get; set; }

        public ISelector EmigrantsSelector { get; set; }

        public IImmigrationReplacer ImmigrationReplacer { get; set; }

        public int MaximumGenerations { get; set; }

        public int MigrationInterval { get; set; }

        public int MigrationRate { get; set; }

        public double MutationProbability { get; set; }

        public IMutator Mutator { get; set; }

        public int PopulationSize { get; set; }

        public int Seed { get; set; }

        public ISelector Selector { get; set; }

        public bool SetSeedRandomly { get; set; }

        public IslandGAParameters()
        {
        }

        public IslandGAParameters(
            ICrossover crossover,
            int elites,
            ISelector emigrantsSelector,
            IImmigrationReplacer immigrationReplacer,
            int maximumGenerations,
            int migrationInterval,
            int migrationRate,
            double mutationProbability,
            IMutator mutator,
            int populationSize,
            int seed,
            ISelector selector,
            bool setSeedRandomly)
        {
            Crossover = crossover;
            Elites = elites;
            EmigrantsSelector = emigrantsSelector;
            ImmigrationReplacer = immigrationReplacer;
            MaximumGenerations = maximumGenerations;
            MigrationInterval = migrationInterval;
            MigrationRate = migrationRate;
            MutationProbability = mutationProbability;
            Mutator = mutator;
            PopulationSize = populationSize;
            Seed = seed;
            Selector = selector;
            SetSeedRandomly = setSeedRandomly;
        }
    }
}
