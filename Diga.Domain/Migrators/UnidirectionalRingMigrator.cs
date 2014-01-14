using Diga.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Migrators
{
    public class UnidirectionalRingMigrator : IMigrator
    {
        public int[] GetMigrationMap(int numberOfPopulations)
        {
            return Enumerable.Range(0, numberOfPopulations).Select(i => (i + 1) % numberOfPopulations).ToArray();
        }
    }
}
