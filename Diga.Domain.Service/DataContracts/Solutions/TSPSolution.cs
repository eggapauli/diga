using System.Runtime.Serialization;

namespace Diga.Domain.Service.DataContracts.Solutions
{
    [DataContract(Namespace = "http://diga.clc.fh-hagenberg/datacontracts")]
    public class TSPSolution : AbstractSolution
    {
        [DataMember]
        public int[] Permutation { get; set; }

        public int Length { get { return Permutation.Length; } }

        public int this[int index]
        {
            get { return Permutation[index]; }
            set { Permutation[index] = value; }
        }
    }
}
