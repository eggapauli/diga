using Diga.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var channelFactory = new ChannelFactory<IDigaService>("basicHttpDigaEndpoint")) {
                var digaService = channelFactory.CreateChannel();
                digaService.GetProblem("IslandGA");
            }
        }
    }
}
