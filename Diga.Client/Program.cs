using Diga.Domain.Service;
using Diga.Domain.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
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
                var problemData = digaService.GetOptimizationTask("SampleTSPProblem");
                var problem = Converter.ConvertFromServiceToDomain(problemData);
                Console.WriteLine(problem);
            }
        }
    }
}
