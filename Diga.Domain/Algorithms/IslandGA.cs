using Diga.Domain.Crossovers;
using Diga.Domain.Selectors;
using Diga.Domain.ImmigrationReplacers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diga.Domain.Mutators;
using Diga.Domain.Parameters;
using Diga.Domain.Contracts;
using System.Runtime.Serialization;

namespace Diga.Domain.Algorithms
{
    public class IslandGA : IAlgorithm
    {
        public IParameters Parameters { get; set; }

        public ISolution BestSolution { get; set; }

        public int Migrations { get; set; }

        public IslandGA()
        {
        }

        public IslandGA(IParameters parameters)
        {
            this.Parameters = parameters;
        }

        public Task CalculateAsync(IProblem problem)
        {
            // TODO implement
            return Task.Delay(1000);
        }

        public IEnumerable<ISolution> ReleaseEmigrants()
        {
            // TODO implement
            yield break;
        }

        public void AddImmigrants(IEnumerable<ISolution> immigrants)
        {
            // TODO implement
        }
    }
}
