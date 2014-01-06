using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Service.Contracts
{
    [ServiceContract(Namespace = "http://diga.clc.fh-hagenberg/services", CallbackContract = typeof(IDigaCallback))]
    public interface IDigaService
    {
        [OperationContract]
        [FaultContract(typeof(TaskNotAddedFault))]
        void AddOptimizationTask(string taskKey, OptimizationTask task);

        [OperationContract]
        [FaultContract(typeof(TaskNotFoundFault))]
        OptimizationTask GetOptimizationTask(string taskKey);

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
