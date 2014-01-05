using Diga.Domain.Contracts;
using Diga.Domain.Solutions;
using System;

namespace Diga.Domain.SolutionCreators
{
    public class RandomTSPSolutionCreator : ISolutionCreator
    {
        public ISolution Apply(Random random, int length)
        {
            return Create(random, length);
        }

        // cf. http://dev.heuristiclab.com/trac/hl/core/browser/stable/HeuristicLab.Encodings.PermutationEncoding/3.3/Creators/RandomPermutationCreator.cs
        public static TSPSolution Create(Random random, int length)
        {
            var solution = new TSPSolution(length);
            Randomize(random, solution);
            return solution;
        }

        private static void Randomize(Random random, TSPSolution solution)
        {
            int length = solution.Length;
            if (length > 1)
            {
                for (int i = length - 1; i > 0; i--)
                {
                    int randIdx = random.Next(i + 1);
                    if (i != randIdx)
                    {
                        int h = solution[i];
                        solution[i] = solution[randIdx];
                        solution[randIdx] = h;
                    }
                }
            }
        }
    }
}
