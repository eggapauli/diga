using Diga.Domain.Service.DataContracts.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Service.DataContracts
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    public class IterationData
    {
        [DataMember]
        public int RemainingIterations { get; private set; }

        [DataMember]
        public IEnumerable<AbstractSolution> Solutions { get; private set; }

        public IterationData()
        {
        }

        public IterationData(int remainingIterations, IEnumerable<AbstractSolution> solutions)
        {
            RemainingIterations = remainingIterations;
            Solutions = solutions;
        }
    }
}
