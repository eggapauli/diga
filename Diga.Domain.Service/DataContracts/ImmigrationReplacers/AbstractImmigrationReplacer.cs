using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.ImmigrationReplacers
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(WorstReplacer))]
    public abstract class AbstractImmigrationReplacer
    {
    }
}
