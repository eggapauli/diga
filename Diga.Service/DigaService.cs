using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Diga.Domain.Service.Contracts;
using DataContracts = Diga.Domain.Service.DataContracts;
using Diga.Domain.Crossovers;
using Diga.Domain.Selectors;
using Diga.Domain.ImmigrationReplacers;
using Diga.Domain.Mutators;
using Diga.Domain.Service;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;

namespace Diga.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DigaService : IDigaService
    {
        public void AddOptimizationTask(string taskKey, DataContracts.OptimizationTask task)
        {
            if (!StateManager.Instance.AddTask(taskKey, task))
            {
                throw new FaultException<TaskNotAddedFault>(new TaskNotAddedFault());
            }
        }

        public DataContracts.OptimizationTask GetOptimizationTask(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }
            else if (task.EndTime != null)
            {
                throw new FaultException<TaskFinishedFault>(new TaskFinishedFault());
            }

            task.StartTime = task.StartTime ?? DateTime.Now;

            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();
            StateManager.Instance.AddWorker(taskKey, channel);
            return task;
        }

        public void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions)
        {
            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();

            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            task.Algorithm.Migrations++;

            if (task.Algorithm.Migrations >= task.Algorithm.Parameters.MaximumMigrations)
            {
                task.EndTime = task.EndTime ?? DateTime.Now;
                channel.Migrate(null);
            }
            else
            {
                // TODO implement migration strategy
                Task.Delay(50).ContinueWith(_ =>
                {
                    channel.Migrate(solutions);
                });
            }
        }

        public async Task SetResultAsync(string taskKey, AbstractSolution bestSolution)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            await StateManager.Instance.UpdateResultAsync(taskKey, bestSolution);
        }

        public async Task<DataContracts.Result> GetResultAsync(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            return await StateManager.Instance.GetResultAsync(taskKey);
        }

        public async Task ClearResultsAsync()
        {
            await StateManager.Instance.ClearResultsAsync();
        }
    }
}
