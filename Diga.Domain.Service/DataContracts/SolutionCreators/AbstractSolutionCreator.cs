using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.SolutionCreators
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(RandomTSPSolutionCreator))]
    public abstract class AbstractSolutionCreator
    {
    }
}

