using System;

namespace Diga.Domain.Contracts
{
    public interface ICrossover
    {
        ISolution Apply(Random random, ISolution solution1, ISolution solution2);
    }
}
