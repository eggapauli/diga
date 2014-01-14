
namespace Diga.Domain.Contracts
{
    public interface IMigrator
    {
        int[] GetMigrationMap(int numberOfPopulations);
    }
}
