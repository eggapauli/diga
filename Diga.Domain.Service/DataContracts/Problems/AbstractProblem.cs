using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Diga.Domain.Service.DataContracts.Problems
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(TSP))]
    public abstract class AbstractProblem
    {
        public abstract bool Maximization { get; }
    }
}
