using System;

namespace Diga.Domain.Contracts
{
    public interface IMutator
    {
        void Apply(Random random, ISolution solution);
    }
}
