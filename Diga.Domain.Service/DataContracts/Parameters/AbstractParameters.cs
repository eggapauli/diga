using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Diga.Domain.Service.DataContracts.Parameters
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(IslandGAParameters))]
    public abstract class AbstractParameters
    {
        [DataMember]
        public int MaximumMigrations { get; set; }
    }
}
