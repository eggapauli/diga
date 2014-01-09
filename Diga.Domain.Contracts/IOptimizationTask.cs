using System;

namespace Diga.Domain.Contracts
{
    public interface IOptimizationTask
    {
        IProblem Problem { get; }

        IAlgorithm Algorithm { get; }

        DateTime? StartTime { get; }

        DateTime? EndTime { get; }
    }
}
