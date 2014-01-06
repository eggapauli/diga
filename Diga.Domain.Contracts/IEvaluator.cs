
namespace Diga.Domain.Contracts
{
    public interface IEvaluator
    {
        void Apply(ISolution solution, IProblem problem);
    }
}

