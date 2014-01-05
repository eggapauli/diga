﻿using Diga.Domain.Service;
using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Diga.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DigaService : IDigaService
    {
        public void AddOptimizationTask(string taskKey, OptimizationTask task)
        {
            if (!StateManager.Instance.AddTask(taskKey, task))
            {
                throw new FaultException<TaskNotAddedFault>(new TaskNotAddedFault());
            }
        }

        public OptimizationTask GetOptimizationTask(string taskKey)
        {
            var task = StateManager.Instance.GetTask(taskKey);
            if (task == null)
            {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();
            StateManager.Instance.AddWorker(taskKey, channel);
            return task;
        }

        public void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions)
        {
            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();

            var task = StateManager.Instance.GetTask(taskKey);
            task.Algorithm.Migrations++;

            if (task.Algorithm.Migrations == task.Algorithm.Parameters.MaximumMigrations)
            {
                channel.Finish();
            }
            else
            {
                // TODO implement migration strategy
                Task.Delay(1000).ContinueWith(_ => channel.Migrate(solutions));
            }
        }

        public void SetResult(string taskKey, AbstractSolution bestSolution)
        {
            // TODO update best solution
        }
    }
}
