using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Service
{
    public class StateManager
    {
        private static Lazy<StateManager> current = new Lazy<StateManager>(() => new StateManager());

        private ConcurrentDictionary<string, OptimizationTask> tasks =
            new ConcurrentDictionary<string, OptimizationTask>();

        private ConcurrentDictionary<string, ConcurrentBag<IDigaCallback>> workers =
            new ConcurrentDictionary<string, ConcurrentBag<IDigaCallback>>();

        public static StateManager Current
        {
            get { return current.Value; }
        }

        public bool AddTask(string key, OptimizationTask task)
        {
            return tasks.TryAdd(key, task);
        }

        public OptimizationTask GetTask(string key)
        {
            OptimizationTask task;
            if (tasks.TryGetValue(key, out task)) {
                return task;
            }
            return null;
        }

        public OptimizationTask RemoveTask(string key)
        {
            OptimizationTask task;
            if (tasks.TryRemove(key, out task)) {
                return task;
            }
            return null;
        }

        public void AddWorker(string key, IDigaCallback worker)
        {
            var list = workers.GetOrAdd(key, new ConcurrentBag<IDigaCallback>());
            list.Add(worker);
        }
    }
}
