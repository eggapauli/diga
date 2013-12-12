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

        int Migrations { get; set; }

        Task CalculateAsync(IProblem problem);

        IEnumerable<ISolution> ReleaseEmigrants();

        void AddImmigrants(IEnumerable<ISolution> immigrants);
    }
}
