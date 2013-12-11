using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
using Diga.Domain.Crossovers;
using Diga.Domain.Selectors;
using Diga.Domain.ImmigrationReplacers;
using Diga.Domain.Mutators;
using Diga.Domain.Service;
using Diga.Domain.Service.DataContracts.Solutions;

namespace Diga.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DigaService : IDigaService
    {
        public OptimizationTask GetOptimizationTask(string key)
        {
            var parameters = new Domain.Parameters.IslandGAParameters(new Domain.Crossovers.MaximalPreservativeCrossover(), 1, new Domain.Selectors.BestSelector(), new Domain.ImmigrationReplacers.WorstReplacer(), 100, 5, 2, 0.1, new Domain.Mutators.InversionManipulator(), 500, 0, new Domain.Selectors.BestSelector(), true);
            var task = new Domain.OptimizationTask(new Domain.Problems.TSP(), new Domain.Algorithms.IslandGA(parameters));

            return (OptimizationTask)Converter.ConvertFromDomainToService(task);
        }

        public void Migrate(string key, IEnumerable<AbstractSolution> solutions)
        {
            throw new NotImplementedException();
        }
    }
}
