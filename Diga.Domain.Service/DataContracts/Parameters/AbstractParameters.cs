using System;
using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Parameters
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    [KnownType(typeof(IslandGAParameters))]
    public abstract class AbstractParameters
    {
        [DataMember]
        public Random Random { get; set; }

        [DataMember]
        public int MaximumMigrations { get; set; }
    }
}
