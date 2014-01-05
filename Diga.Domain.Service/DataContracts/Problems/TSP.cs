using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Problems
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    public class TSP : AbstractProblem
    {
        [DataMember]
        public double[][] Coordinates { get; set; }
    }
}
