using Diga.Domain.Service.DataContracts.Parameters;
using Diga.Domain.Service.DataContracts.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Diga.Domain.Service.DataContracts.Algorithms
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(IslandGA))]
    public class AbstractAlgorithm
    {
        [DataMember]
        public AbstractParameters Parameters { get; set; }

        [DataMember]
        public AbstractSolution BestSolution { get; set; }

        [DataMember]
        public int Migrations { get; set; }
    }
}
