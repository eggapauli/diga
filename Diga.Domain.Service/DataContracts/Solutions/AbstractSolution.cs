using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Diga.Domain.Service.DataContracts.Solutions
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(TSPSolution))]
    public abstract class AbstractSolution
    {
        [DataMember]
        public double Quality { get; set; }
    }
}
