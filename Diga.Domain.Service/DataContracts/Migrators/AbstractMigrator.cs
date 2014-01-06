using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Migrators
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(UnidirectionalRingMigrator))]
    public abstract class AbstractMigrator
    {
    }
}
