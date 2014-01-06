using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(DigaService)))
            {
                host.Open();
                Console.WriteLine("Diga service started ...");
                Console.WriteLine("Press enter to stop the service.");
                Console.ReadLine();
            }
        }
    }
}
