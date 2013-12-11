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
    public interface IDigaCallback
    {
        [OperationContract(IsOneWay = true)]
        void Migrate(string key, IterationData iterationData);
    }
}
