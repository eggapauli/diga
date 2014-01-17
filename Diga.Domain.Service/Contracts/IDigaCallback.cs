using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Diga.Domain.Service.Contracts
{
    public interface IDigaCallback
    {
        [OperationContract(IsOneWay = true)]
        void StartWork(OptimizationTask task);

        [OperationContract(IsOneWay = true)]
        void Migrate(IEnumerable<AbstractSolution> solutions);

        Task<OptimizationTask> WaitForStartAsync();

        Task<IEnumerable<AbstractSolution>> WaitForMigrationAsync();

    }
}
