using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
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
        void AddOptimizationTask(string key, OptimizationTask task);

        [OperationContract]
        OptimizationTask GetOptimizationTask(string key);

        [OperationContract]
        void Migrate(string key, IEnumerable<AbstractSolution> solutions);
    }
}
