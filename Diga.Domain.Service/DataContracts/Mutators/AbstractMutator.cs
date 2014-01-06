using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Mutators
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(InversionManipulator))]
    public abstract class AbstractMutator
    {
    }
}
