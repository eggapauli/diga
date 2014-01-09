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
        void Migrate(IEnumerable<AbstractSolution> solutions);

        Task<IEnumerable<AbstractSolution>> WaitForMigrationAsync();
    }
}
