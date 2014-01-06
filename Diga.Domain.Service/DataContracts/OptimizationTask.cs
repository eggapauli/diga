using Diga.Domain.Service.DataContracts.Algorithms;
using Diga.Domain.Service.DataContracts.Problems;
using System;
using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    public class OptimizationTask
    {
        [DataMember]
        public AbstractProblem Problem { get; set; }

        [DataMember]
        public AbstractAlgorithm Algorithm { get; set; }

        public DateTime StartTime { get; set; }
    }
}
