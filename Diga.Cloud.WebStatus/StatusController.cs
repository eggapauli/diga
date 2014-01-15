using Diga.Domain.Service.Contracts;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Http;
using DataContracts = Diga.Domain.Service.DataContracts;
using Svc = Diga.Domain.Service;

namespace Diga.Cloud.WebStatus
{
    public class StatusController : ApiController
    {
        private readonly IDigaCallback digaCallback = new DummyCallback();

        public IEnumerable<string> GetAllTaskKeys()
        {
            using (var channelFactory = new DuplexChannelFactory<Svc.Contracts.IDigaService>(new InstanceContext(digaCallback), "DigaService_DualHttpEndpoint"))
            {
                var digaService = channelFactory.CreateChannel();
                return digaService.GetAllOptimizationTaskKeys();
            }
        }

        public DataContracts.OptimizationTask GetTaskById(string taskKey)
        {
            using (var channelFactory = new DuplexChannelFactory<Svc.Contracts.IDigaService>(new InstanceContext(digaCallback), "DigaService_DualHttpEndpoint"))
            {
                var digaService = channelFactory.CreateChannel();
                return digaService.GetOptimizationTask(taskKey);
            }
        }
    }
}
