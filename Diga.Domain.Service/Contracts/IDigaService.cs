using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Diga.Domain.Service.Contracts
{
    [ServiceContract(Namespace = "http://diga.clc.fh-hagenberg/services", CallbackContract = typeof(IDigaCallback))]
    public interface IDigaService
    {
        [OperationContract]
        [FaultContract(typeof(TaskNotAddedFault))]
        void AddOptimizationTask(OptimizationTask task);

        [OperationContract]
        [FaultContract(typeof(TaskNotFoundFault))]
        [FaultContract(typeof(TaskFinishedFault))]
        void ApplyForCalculatingOptimizationTask(string taskKey);

        [OperationContract]
        [FaultContract(typeof(TaskNotFoundFault))]
        [FaultContract(typeof(TaskFinishedFault))]
        void StartOptimizationTask(string taskKey);

        [OperationContract]
        [FaultContract(typeof(TaskNotFoundFault))]
        void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions);

        [OperationContract]
        [FaultContract(typeof(TaskNotFoundFault))]
        Task SetResultAsync(string taskKey, AbstractSolution bestSolution);

        [OperationContract]
        [FaultContract(typeof(TaskNotFoundFault))]
        Task<Result> GetResultAsync(string taskKey);

        [OperationContract]
        Task ClearResultsAsync();
    }
}
