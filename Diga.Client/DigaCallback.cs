using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Solutions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Diga.Client
{
    public class DigaCallback : IDigaCallback
    {
        private TaskCompletionSource<IEnumerable<AbstractSolution>> tcs;

        public void Migrate(IEnumerable<AbstractSolution> iterationData)
        {
            tcs.SetResult(iterationData);
        }

        public Task<IEnumerable<AbstractSolution>> WaitForMigrationAsync()
        {
            tcs = new TaskCompletionSource<IEnumerable<AbstractSolution>>();
            return tcs.Task;
        }
    }
}
