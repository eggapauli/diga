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
        public void AddOptimizationTask(string taskKey, OptimizationTask task)
        {
            throw new NotImplementedException();
        }

        public OptimizationTask GetOptimizationTask(string taskKey)
        {
            throw new NotImplementedException();
        }

        public void Migrate(string taskKey, IEnumerable<AbstractSolution> solutions)
        {
            throw new NotImplementedException();
        }

        public void SetResult(string taskKey, AbstractSolution bestSolution)
        {
            throw new NotImplementedException();
        }
    }
}
