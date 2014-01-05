using System;

namespace Diga.Domain.Contracts
{
    public interface IParameters
    {
        Random Random { get; set; }
        int MaximumMigrations { get; set; }
    }
}
