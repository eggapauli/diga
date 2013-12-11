using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Contracts
{
    public interface IOptimizationTask
    {
        IProblem Problem { get; }

        IAlgorithm Algorithm { get; }
    }
}
