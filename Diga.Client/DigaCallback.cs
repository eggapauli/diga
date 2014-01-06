using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Solutions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Diga.Client
{
    public class DigaCallback : IDigaCallback
    {
        private TaskCompletionSource<IEnumerable<AbstractSolution>> tcs = new TaskCompletionSource<IEnumerable<AbstractSolution>>();

        private CancellationTokenSource cts = new CancellationTokenSource();

        public CancellationToken FinishToken
        {
            get { return cts.Token; }
        }

        #region Operation Contracts

        public void Migrate(IEnumerable<AbstractSolution> iterationData)
        {
            tcs.SetResult(iterationData);
        }

        public void Finish()
        {
            cts.Cancel();
        }

        #endregion

        public void Reset()
        {
            if (cts != null)
            {
                cts.Dispose();
            }
            cts = new CancellationTokenSource();
        }

        public Task<IEnumerable<AbstractSolution>> WaitForMigrationAsync()
        {
            if (tcs.Task.IsCompleted)
            {
                // `Migrate` has been called from the service before user called this method
                var task = tcs.Task;
                tcs = new TaskCompletionSource<IEnumerable<AbstractSolution>>();
                return task;
            }
            return tcs.Task;
        }
    }
}
