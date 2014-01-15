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

namespace Diga.Cloud.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DigaService : IDigaService
    {
        private static DigaServiceLogic logic = new DigaServiceLogic();

        public void AddOptimizationTask(string taskKey, DataContracts.OptimizationTask task)
        {
            logic.AddOptimizationTask(taskKey, task);
        }

        public DataContracts.OptimizationTask GetOptimizationTask(string taskKey)
        {
            return logic.GetOptimizationTask(taskKey);
        }

        public IEnumerable<string> GetAllOptimizationTaskKeys()
        {
            return logic.GetAllOptimizationTaskKeys();
        }

        public void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions)
        {
            logic.Migrate(taskKey, solutions);
        }

        public async Task SetResultAsync(string taskKey, AbstractSolution bestSolution)
        {
            await logic.SetResultAsync(taskKey, bestSolution);
        }

        public async Task<DataContracts.Result> GetResultAsync(string taskKey)
        {
            return await logic.GetResultAsync(taskKey);
        }

        public async Task ClearResultsAsync()
        {
            await logic.ClearResultsAsync();
        }
    }
}
