using System;
using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    public class Result
    {
        [DataMember]
        public TimeSpan RunDuration { get; set; }

        [DataMember]
        public double BestQuality { get; set; }

        [DataMember]
        public int NumberOfWorkers { get; set; }

        public Result(TimeSpan runDuration, double bestQuality, int numberOfWorkers)
        {
            RunDuration = runDuration;
            BestQuality = bestQuality;
            NumberOfWorkers = numberOfWorkers;
        }
    }
}
