using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Crossovers
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(MaximalPreservativeCrossover))]
    public abstract class AbstractCrossover
    {
    }
}
