using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Selectors
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(BestSelector))]
    public abstract class AbstractSelector
    {
    }
}
