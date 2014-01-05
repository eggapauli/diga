using System.Collections.Generic;

namespace Diga.Domain.Contracts
{
    public interface ISelector
    {
        IEnumerable<ISolution> Apply(IEnumerable<ISolution> solutions, int numberOfSelectedSolutions, bool maximization = false);
    }
}
