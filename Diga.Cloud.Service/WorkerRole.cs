using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.ServiceModel;
using Diga.Domain.Service.Contracts;

namespace Diga.Cloud.Service
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            Trace.TraceInformation("Diga.Cloud.Service entry point called", "Information");

            while (true) {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            CreateServiceHost();
            return base.OnStart();
        }

        private ServiceHost serviceHost;
        private void CreateServiceHost()
        {
            serviceHost = new ServiceHost(typeof(DigaService));

            var protocolMap = new Dictionary<string, string> {
                { "http", "http" },
                { "tcp", "net.tcp" }
            };

            var internalEndpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DefaultInternalEndpoint"];
            var internalEndpointAddress = string.Format("{0}://{1}/diga", protocolMap[internalEndpoint.Protocol], internalEndpoint.IPEndpoint);

            var externalEndpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DefaultExternalEndpoint"];
            var externalEndpointAddress = string.Format("{0}://{1}/diga", protocolMap[externalEndpoint.Protocol], externalEndpoint.IPEndpoint);

            serviceHost.AddServiceEndpoint(
                typeof(IDigaStatusService),
                new NetTcpBinding(SecurityMode.None),
                internalEndpointAddress);

            serviceHost.AddServiceEndpoint(
                typeof(IDigaService),
                new NetHttpBinding(BasicHttpSecurityMode.None),
                externalEndpointAddress);

            Trace.TraceInformation(string.Format("Starting status service at {0}", internalEndpointAddress), "Information");
            Trace.TraceInformation(string.Format("Starting calculation service at {0}", externalEndpointAddress), "Information");

            serviceHost.Open();
        }
    }
}
