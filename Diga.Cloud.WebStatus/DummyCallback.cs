using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Solutions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diga.Cloud.WebStatus
{
    internal class DummyCallback : IDigaCallback
    {
        public void Migrate(IEnumerable<AbstractSolution> solutions)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AbstractSolution>> WaitForMigrationAsync()
        {
            throw new NotImplementedException();
        }
    }
}
