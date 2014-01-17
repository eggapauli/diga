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
        public string TaskKey { get; private set; }

        public IProblem Problem { get; private set; }

        public IAlgorithm Algorithm { get; private set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public OptimizationTask()
        {
        }

        public OptimizationTask(string taskKey, TSP problem, IslandGA algorithm)
        {
            TaskKey = taskKey;
            Problem = problem;
            Algorithm = algorithm;
        }
    }
}
