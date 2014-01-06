using Diga.Domain.Algorithms;
using Diga.Domain.Contracts;
using Diga.Domain.Problems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain
{
    public class OptimizationTask : IOptimizationTask
    {
        public IProblem Problem { get; private set; }

        public IAlgorithm Algorithm { get; private set; }

        public DateTime StartTime { get; set; }

        public OptimizationTask()
        {
        }

        public OptimizationTask(TSP problem, IslandGA algorithm)
        {
            this.Problem = problem;
            this.Algorithm = algorithm;
        }
    }
}
