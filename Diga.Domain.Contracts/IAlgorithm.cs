using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Contracts
{
    public interface IAlgorithm
    {
        IParameters Parameters { get; }

        ISolution BestSolution { get; set; }

        Task CalculateAsync(IProblem problem);

        IEnumerable<ISolution> ReleaseSolutionsForMigration();
    }
}
