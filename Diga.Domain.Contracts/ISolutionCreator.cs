using System;

namespace Diga.Domain.Contracts
{
    public interface ISolutionCreator
    {
        ISolution Apply(Random random, int length);
    }
}
