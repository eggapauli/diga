using Diga.Domain.Contracts;
using Diga.Domain.Problems;
using Diga.Domain.Solutions;
using System;

namespace Diga.Domain.Evaluators
{
    public class TSPSolutionEvaluator : IEvaluator
    {
        public void Apply(ISolution solution, IProblem problem)
        {
            var sol = solution as TSPSolution;
            if (sol == null) throw new ArgumentException("The given solution has the wrong type.", "solution");
            var prob = problem as TSP;
            if (prob == null) throw new ArgumentException("The given problem has the wrong type.", "problem");
            Evaluate(sol, prob);
        }

        // cf. http://dev.heuristiclab.com/trac/hl/core/browser/stable/HeuristicLab.Encodings.PermutationEncoding/3.3/Manipulators/InversionManipulator.cs
        public static void Evaluate(TSPSolution solution, TSP problem)
        {
            double[][] c = problem.Coordinates;
            int[] p = solution.Permutation;
            double length = 0;
            for (int i = 0; i < p.Length - 1; i++)
                length += CalculateDistance(c[p[i]][0], c[p[i]][1], c[p[i + 1]][0], c[p[i + 1]][1]);
            length += CalculateDistance(c[p[p.Length - 1]][0], c[p[p.Length - 1]][1], c[p[0]][0], c[p[0]][1]);
            solution.Quality = length;
        }

        private static double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }
    }
}
