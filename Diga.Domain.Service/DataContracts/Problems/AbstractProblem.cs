using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Problems
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(TSP))]
    public abstract class AbstractProblem
    {
        [DataMember]
        public bool Maximization { get; set; }
    }
}
