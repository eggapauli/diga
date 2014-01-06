using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Diga.Domain.Service.DataContracts.Crossovers
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(MaximalPreservativeCrossover))]
    public abstract class AbstractCrossover
    {
    }
}
