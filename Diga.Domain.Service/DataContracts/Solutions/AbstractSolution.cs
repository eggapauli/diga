using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Solutions
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(TSPSolution))]
    public abstract class AbstractSolution
    {
        [DataMember]
        public double Quality { get; set; }
    }
}
