using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Diga.Client
{
    public class DigaCallback : IDigaCallback
    {
        private TaskCompletionSource<OptimizationTask> tcsStart;
        private TaskCompletionSource<IEnumerable<AbstractSolution>> tcsMigrate;

        public void StartWork(OptimizationTask task)
        {
            tcsStart.SetResult(task);
        }

        public void Migrate(IEnumerable<AbstractSolution> iterationData)
        {
            tcsMigrate.SetResult(iterationData);
        }

        public Task<OptimizationTask> WaitForStartAsync()
        {
            tcsStart = new TaskCompletionSource<OptimizationTask>();
            return tcsStart.Task;
        }

        public Task<IEnumerable<AbstractSolution>> WaitForMigrationAsync()
        {
            tcsMigrate = new TaskCompletionSource<IEnumerable<AbstractSolution>>();
            return tcsMigrate.Task;
        }

    }
}
