using Diga.Domain.Contracts;
using System.Linq;

namespace Diga.Domain.Solutions
{
    public class TSPSolution : ISolution
    {
        public double Quality { get; set; }

        public int[] Permutation { get; set; }

        public int Length { get { return Permutation.Length; } }

        public int this[int index]
        {
            get { return Permutation[index]; }
            set { Permutation[index] = value; }
        }

        public TSPSolution() { }

        public TSPSolution(int length)
        {
            Permutation = Enumerable.Range(0, length).ToArray();
        }

        public TSPSolution(int[] permutation)
        {
            Permutation = permutation;
        }
    }
}