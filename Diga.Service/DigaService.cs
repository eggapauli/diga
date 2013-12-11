using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
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
        public void AddOptimizationTask(string key, OptimizationTask task)
        {
            if (!StateManager.Current.AddTask(key, task)) {
                throw new FaultException<TaskNotAddedFault>(new TaskNotAddedFault());
            }
        }

        public OptimizationTask GetOptimizationTask(string key)
        {
            var task = StateManager.Current.GetTask(key);
            if (task == null) {
                throw new FaultException<TaskNotFoundFault>(new TaskNotFoundFault());
            }

            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();
            StateManager.Current.AddWorker(key, channel);
            return task;
        }

        public void Migrate(string key, IEnumerable<AbstractSolution> solutions)
        {
            // TODO implement migration strategy
            // TODO decrease remaining iterations (StateManager)
            var channel = OperationContext.Current.GetCallbackChannel<IDigaCallback>();
            channel.Migrate(key, new IterationData(10, solutions));
        }
    }
}
