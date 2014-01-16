using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.FaultContracts;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Web.Http;
using DataContracts = Diga.Domain.Service.DataContracts;
using Svc = Diga.Domain.Service;

namespace Diga.Cloud.WebStatus
{
    public class StatusController : ApiController
    {
        public IEnumerable<string> GetAllTaskKeys()
        {
            using (var channelFactory = GetChannelFactory())
            {
                var digaService = channelFactory.CreateChannel();
                return digaService.GetAllOptimizationTaskKeys();
            }
        }

        public DataContracts.OptimizationTask GetTaskById(string taskKey)
        {
            using (var channelFactory = GetChannelFactory())
            {
                var digaService = channelFactory.CreateChannel();
                try {
                    return digaService.GetOptimizationTask(taskKey);
                }
                catch (FaultException<TaskNotFoundFault>) {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
            }
        }

        private ChannelFactory<Svc.Contracts.IDigaStatusService> GetChannelFactory()
        {
            var instance = RoleEnvironment.Roles["Diga.Cloud.Service"].Instances[0];
            var internalEP = instance.InstanceEndpoints["DefaultInternalEndpoint"];

            var binding = new NetTcpBinding(SecurityMode.None);
            string endpoint = string.Format("net.tcp://{0}/diga", internalEP.IPEndpoint);

            return new ChannelFactory<Svc.Contracts.IDigaStatusService>(binding, endpoint);
        }
    }
}
