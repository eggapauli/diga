using Diga.Contracts.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Contracts.Services
{
    [ServiceContract(Namespace = "http://diga.clc.fh-hagenberg")]
    public interface IDigaService
    {
        [OperationContract]
        IParameters GetProblem(string problemType);

        [OperationContract]
        void Migrate(IEnumerable<ISolution> solutions);
    }
}
