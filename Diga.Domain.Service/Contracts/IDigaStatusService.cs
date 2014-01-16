using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.FaultContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Service.Contracts
{
    [ServiceContract(Namespace = "http://diga.clc.fh-hagenberg/services")]
    public interface IDigaStatusService
    {
        [OperationContract]
        [FaultContract(typeof(TaskNotFoundFault))]
        OptimizationTask GetOptimizationTask(string taskKey);

        [OperationContract]
        IEnumerable<string> GetAllOptimizationTaskKeys();
    }
}
