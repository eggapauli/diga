using Diga.Domain.Service.DataContracts.Crossovers;
using Diga.Domain.Service.DataContracts.Evaluators;
using Diga.Domain.Service.DataContracts.ImmigrationReplacers;
using Diga.Domain.Service.DataContracts.Migrators;
using Diga.Domain.Service.DataContracts.Mutators;
using Diga.Domain.Service.DataContracts.Selectors;
using Diga.Domain.Service.DataContracts.SolutionCreators;
using System.Runtime.Serialization;

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
        public AbstractEvaluator Evaluator { get; set; }

        [DataMember]
        public AbstractImmigrationReplacer ImmigrationReplacer { get; set; }

        [DataMember]
        public int MigrationInterval { get; set; }

        [DataMember]
        public double MigrationRate { get; set; }

        [DataMember]
        public AbstractMigrator Migrator { get; set; }

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

        [DataMember]
        public AbstractSolutionCreator SolutionCreator { get; set; }
    }
}
