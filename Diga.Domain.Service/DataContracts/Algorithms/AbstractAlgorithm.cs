using Diga.Domain.Service.DataContracts.Parameters;
using Diga.Domain.Service.DataContracts.Solutions;
using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Algorithms
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(IslandGA))]
    public abstract class AbstractAlgorithm
    {
        [DataMember]
        public AbstractParameters Parameters { get; set; }

        [DataMember]
        public AbstractSolution BestSolution { get; set; }

        [DataMember]
        public int Migrations { get; set; }
    }
}
