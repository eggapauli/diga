using Diga.Domain.Service;
using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Client
{
    public delegate void OnMigrateEvent(string key, IterationData solutions);

    public class DigaCallback : IDigaCallback
    {
        public event OnMigrateEvent OnMigrate;

        public void Migrate(string key, IterationData iterationData)
        {
            var onMigrate = OnMigrate;
            if (onMigrate != null) {
                onMigrate(key, iterationData);
            }
        }
    }
}
