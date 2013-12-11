using Diga.Domain.Service.DataContracts.Crossovers;
using Diga.Domain.Service.DataContracts.ImmigrationReplacers;
using Diga.Domain.Service.DataContracts.Mutators;
using Diga.Domain.Service.DataContracts.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Service.DataContracts.Parameters
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    public class IslandGAParameters : AbstractParameters
    {
        [DataMember]
        public AbstractCrossover Crossover { get; set; }

        [DataMember]
        public int Elites { get; set; }

        [DataMember]
        public AbstractSelector EmigrantsSelector { get; set; }

        [DataMember]
        public AbstractImmigrationReplacer ImmigrationReplacer { get; set; }

        [DataMember]
        public int MaximumGenerations { get; set; }

        [DataMember]
        public int MigrationInterval { get; set; }

        [DataMember]
        public int MigrationRate { get; set; }

        [DataMember]
        public double MutationProbability { get; set; }

        [DataMember]
        public AbstractMutator Mutator { get; set; }

        [DataMember]
        public int PopulationSize { get; set; }

        [DataMember]
        public int Seed { get; set; }

        [DataMember]
        public AbstractSelector Selector { get; set; }

        [DataMember]
        public bool SetSeedRandomly { get; set; }
    }
}
