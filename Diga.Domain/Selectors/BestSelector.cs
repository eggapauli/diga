using Diga.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Diga.Domain.Selectors
{
    public class BestSelector : ISelector
    {
        public IEnumerable<ISolution> Apply(IEnumerable<ISolution> solutions, int numberOfSelectedSolutions, bool maximization = false)
        {
            return Select(solutions, numberOfSelectedSolutions, maximization);
        }

        public static IEnumerable<ISolution> Select(IEnumerable<ISolution> solutions, int numberOfSelectedSolutions, bool maximization = false)
        {
            var orderedSolutions = maximization
                ? solutions.OrderByDescending(x => x.Quality)
                : solutions.OrderBy(x => x.Quality);

            var best = new List<ISolution>(numberOfSelectedSolutions);
            do
            {
                best.AddRange(orderedSolutions.Take(numberOfSelectedSolutions - best.Count));
            } while (best.Count < numberOfSelectedSolutions);
            return best;
        }
    }
}
