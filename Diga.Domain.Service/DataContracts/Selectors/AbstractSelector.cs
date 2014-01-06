using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Diga.Domain.Service.DataContracts.Selectors
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(BestSelector))]
    public abstract class AbstractSelector
    {
    }
}
