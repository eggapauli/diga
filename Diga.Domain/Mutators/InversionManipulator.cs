using Diga.Domain.Contracts;
using Diga.Domain.Solutions;
using System;

namespace Diga.Domain.Mutators
{
    public class InversionManipulator : IMutator
    {
        public void Apply(Random random, ISolution solution)
        {
            var sol = solution as TSPSolution;
            if (sol == null) throw new ArgumentException("The given solution has the wrong type.", "solution");
            Mutate(random, sol);
        }

        // cf. http://dev.heuristiclab.com/trac/hl/core/browser/stable/HeuristicLab.Encodings.PermutationEncoding/3.3/Manipulators/InversionManipulator.cs
        public static void Mutate(Random random, TSPSolution solution)
        {
            int length = solution.Length;
            int idx1, idx2;

            idx1 = random.Next(length - 1);
            do { idx2 = random.Next(length - 1); } while (idx2 == idx1);
            if (idx2 < idx1) { int h = idx1; idx1 = idx2; idx2 = h; }
            for (int i = 0; i <= (idx2 - idx1) / 2; i++)
            {
                int h = solution[idx1 + i];
                solution[idx1 + i] = solution[idx2 - i];
                solution[idx2 - i] = h;
            }
        }
    }
}
