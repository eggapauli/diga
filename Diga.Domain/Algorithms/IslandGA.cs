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
using Diga.Contracts.Optimization;

namespace Diga.Domain.Algorithms
{
    public class IslandGA : IAlgorithm
    {
        public IslandGA(IslandGAParameters parameters)
        {
            this.Parameters = parameters;
        }

        public IParameters Parameters { get; set; }

        public ISolution BestSolution { get; set; }

        #region Possible Parameters

        public IEnumerable<ICrossover> Crossovers
        {
            get
            {
                yield return new MaximalPreservativeCrossover();
            }
        }

        public IEnumerable<ISelector> EmigrantsSelectors
        {
            get
            {
                yield return new BestSelector();
            }
        }

        public IEnumerable<IImmigrationReplacer> ImmigrationReplacers
        {
            get
            {
                yield return new WorstReplacer();
            }
        }

        public IEnumerable<IMutator> Mutators
        {
            get
            {
                yield return new InversionManipulator();
            }
        }

        public IEnumerable<ISelector> Selectors
        {
            get
            {
                yield return new BestSelector();
            }
        }

        #endregion
    }
}
