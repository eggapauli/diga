using Diga.Contracts.Optimization;
using Diga.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Service
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
    public class DigaService : IDigaService
    {
        public IParameters GetProblem(string problemType)
        {
            throw new NotImplementedException();
        }

        public void Migrate(IEnumerable<ISolution> solutions)
        {
            throw new NotImplementedException();
        }
    }
}
