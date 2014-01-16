using Diga.Domain.Service;
using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Diga.Cloud.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DigaService : IDigaService, IDigaStatusService
    {
        private static DigaServiceLogic logic = new DigaServiceLogic();

        public void AddOptimizationTask(string taskKey, OptimizationTask task)
        {
            logic.AddOptimizationTask(taskKey, task);
        }

        public OptimizationTask ApplyForCalculatingOptimizationTask(string taskKey)
        {
            return logic.ApplyForCalculatingOptimizationTask(taskKey);
        }

        public void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions)
        {
            logic.Migrate(taskKey, solutions);
        }

        public async Task SetResultAsync(string taskKey, AbstractSolution bestSolution)
        {
            await logic.SetResultAsync(taskKey, bestSolution);
        }

        public async Task<Result> GetResultAsync(string taskKey)
        {
            return await logic.GetResultAsync(taskKey);
        }

        public async Task ClearResultsAsync()
        {
            await logic.ClearResultsAsync();
        }

        public OptimizationTask GetOptimizationTask(string taskKey)
        {
            return logic.GetOptimizationTask(taskKey);
        }

        public IEnumerable<string> GetAllOptimizationTaskKeys()
        {
            return logic.GetAllOptimizationTaskKeys();
        }
    }
}
