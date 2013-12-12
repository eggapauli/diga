﻿using Diga.Domain.Service.DataContracts.Algorithms;
using Diga.Domain.Service.DataContracts.Problems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Service.DataContracts
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    public class OptimizationTask
    {
        [DataMember]
        public AbstractProblem Problem { get; set; }

        [DataMember]
        public AbstractAlgorithm Algorithm { get; set; }
    }
}