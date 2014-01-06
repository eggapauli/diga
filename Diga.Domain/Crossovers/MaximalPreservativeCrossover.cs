using Diga.Domain.Contracts;
using Diga.Domain.Solutions;
using System;

namespace Diga.Domain.Crossovers
{
    public class MaximalPreservativeCrossover : ICrossover
    {
        public ISolution Apply(Random random, ISolution solution1, ISolution solution2)
        {
            var mother = solution1 as TSPSolution;
            if (mother == null) throw new ArgumentException("The given solution has the wrong type.", "solution1");
            var father = solution2 as TSPSolution;
            if (father == null) throw new ArgumentException("The given solution has the wrong type.", "solution2");
            if (mother.Length != father.Length) throw new ArgumentException("The given solutions have different lengths.");
            if (mother.Length < 4) throw new ArgumentException("The given solutions must be at least of size 4.");
            return Cross(random, mother, father);
        }

        // cf. http://dev.heuristiclab.com/trac/hl/core/browser/stable/HeuristicLab.Encodings.PermutationEncoding/3.3/Crossovers/MaximalPreservativeCrossover.cs
        public static TSPSolution Cross(Random random, TSPSolution solution1, TSPSolution solution2)
        {
            int length = solution1.Length;
            int[] result = new int[length];
            bool[] numberCopied = new bool[length];
            int currIdx, idx1, idx2, subsegmentLength;

            subsegmentLength = random.Next(3, Math.Max(length / 3, 4));
            idx1 = random.Next(length);
            idx2 = idx1 + subsegmentLength;
            if (idx2 >= length) idx2 -= length;

            currIdx = idx1;
            do
            {
                result[currIdx] = solution1[currIdx];
                numberCopied[result[currIdx]] = true;
                currIdx++;
                if (currIdx >= length) currIdx -= length;
            } while (currIdx != idx2);

            int[] invSolution1 = new int[length];
            int[] invSolution2 = new int[length];
            for (int i = 0; i < length; i++)
            {
                invSolution1[solution1[i]] = i;
                invSolution2[solution2[i]] = i;
            }

            int prevIdx = currIdx > 0 ? currIdx - 1 : length - 1;
            do
            {
                int s2Follower = GetFollower(solution2, invSolution2[result[prevIdx]]);
                if (!numberCopied[s2Follower]) result[currIdx] = s2Follower;
                else
                {
                    int s1Follower = GetFollower(solution1, invSolution1[result[prevIdx]]);
                    if (!numberCopied[s1Follower])
                        result[currIdx] = s1Follower;
                    else
                    {
                        int tmpIdx = currIdx;
                        for (int i = 0; i < solution2.Length; i++)
                        {
                            if (!numberCopied[solution2[tmpIdx]])
                            {
                                result[currIdx] = solution2[tmpIdx];
                                break;
                            }
                            tmpIdx++;
                            if (tmpIdx >= solution2.Length) tmpIdx = 0;
                        }
                    }
                }
                numberCopied[result[currIdx]] = true;
                prevIdx = currIdx;
                currIdx++;
                if (currIdx >= length) currIdx -= length;
            } while (currIdx != idx1);
            return new TSPSolution(result);
        }

        private static int GetFollower(TSPSolution solution, int index)
        {
            return solution[(index + 1) % solution.Length];
        }
    }
}
