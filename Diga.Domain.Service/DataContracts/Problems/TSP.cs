using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Service.DataContracts.Problems
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    public class TSP : AbstractProblem
    {
        [DataMember]
        public override bool Maximization
        {
            get { return false; }
        }
    }
}
