using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Diga.Cloud.WebStatus
{
    public class WorkerRole : RoleEntryPoint
    {
        private IDisposable app = null;

        public override void Run()
        {
            Trace.TraceInformation("WebStatus entry point called", "Information");
            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DefaultExternalEndpoint"];
            string baseUri = string.Format("{0}://{1}", endpoint.Protocol, endpoint.IPEndpoint);

            Trace.TraceInformation(string.Format("Starting OWIN at {0}", baseUri), "Information");

            app = WebApp.Start<Startup>(new StartOptions(baseUri));

            return base.OnStart();
        }

        public override void OnStop()
        {
            if (app != null)
                app.Dispose();
            base.OnStop();
        }
    }
}
