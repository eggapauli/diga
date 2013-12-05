using Diga.Contracts.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Parameters
{
    public class IslandGAParameters : IParameters
    {
        public string Crossover { get; set; }

        public int Elites { get; set; }

        public string EmigrantsSelector { get; set; }

        public string ImmigrationReplacer { get; set; }

        public int MaximumGenerations { get; set; }

        public int MigrationInterval { get; set; }

        public int MigrationRate { get; set; }

        public double MutationProbability { get; set; }

        public string Mutator { get; set; }

        public int PopulationSize { get; set; }

        public int Seed { get; set; }

        public string Selector { get; set; }

        public bool SetSeedRandomly { get; set; }
    }
}
