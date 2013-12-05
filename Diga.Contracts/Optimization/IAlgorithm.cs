using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diga.Contracts.Optimization;

namespace Diga.Contracts.Optimization
{
    public interface IAlgorithm
    {
        IParameters Parameters { get; }

        ISolution BestSolution { get; set; }

        #region Possible Parameters

        IEnumerable<ICrossover> Crossovers { get; }

        IEnumerable<ISelector> EmigrantsSelectors { get; }

        IEnumerable<IImmigrationReplacer> ImmigrationReplacers { get; }

        IEnumerable<IMutator> Mutators { get; }

        IEnumerable<ISelector> Selectors { get; }

        #endregion
    }
}
