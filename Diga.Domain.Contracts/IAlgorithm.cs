using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diga.Domain.Contracts
{
    public interface IAlgorithm
    {
        IParameters Parameters { get; }

        ISolution BestSolution { get; set; }

        int Migrations { get; set; }

        Task InitializeAsync(IProblem problem);

        Task CalculateAsync(IProblem problem);

        IEnumerable<ISolution> ReleaseEmigrants(IProblem problem);

        void AddImmigrants(IEnumerable<ISolution> immigrants, IProblem problem);
    }
}
