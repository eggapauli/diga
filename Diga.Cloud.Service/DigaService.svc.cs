using Diga.Domain.Service;
using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.FaultContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Diga.Cloud.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DigaService : IDigaService
    {
        public void AddOptimizationTask(string key, OptimizationTask task)
        {
            throw new NotImplementedException();
        }

        public OptimizationTask GetOptimizationTask(string key)
        {
            throw new NotImplementedException();
        }

        public void Migrate(string key, IEnumerable<AbstractSolution> solutions)
        {
            throw new NotImplementedException();
        }
    }
}
