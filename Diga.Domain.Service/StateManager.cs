using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Diga.Domain.Service.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Diga.Domain.Service
{
    public class StateManager
    {
        private static Lazy<StateManager> instance = new Lazy<StateManager>(() => new StateManager());

        private ConcurrentDictionary<string, DataContracts.OptimizationTask> tasks =
            new ConcurrentDictionary<string, DataContracts.OptimizationTask>();

        private ConcurrentDictionary<string, ConcurrentBag<IRemoteWorker>> workers =
            new ConcurrentDictionary<string, ConcurrentBag<IRemoteWorker>>();

        private ConcurrentDictionary<string, ConcurrentDictionary<IRemoteWorker, IEnumerable<AbstractSolution>>> migrations =
            new ConcurrentDictionary<string, ConcurrentDictionary<IRemoteWorker, IEnumerable<AbstractSolution>>>();

        public static StateManager Instance
        {
            get { return instance.Value; }
        }

        public bool AddTask(DataContracts.OptimizationTask task)
        {
            task.StartTime = null;
            task.EndTime = null;
            return tasks.TryAdd(task.TaskKey, task);
        }

        public DataContracts.OptimizationTask GetTask(string key)
        {
            DataContracts.OptimizationTask task;
            if (tasks.TryGetValue(key, out task))
            {
                return task;
            }
            return null;
        }

        public IEnumerable<string> GetAllTaskKeys()
        {
            return tasks.Keys;
        }

        public DataContracts.OptimizationTask RemoveTask(string key)
        {
            DataContracts.OptimizationTask task;
            if (tasks.TryRemove(key, out task))
            {
                return task;
            }
            return null;
        }

        public void AddWorker(string key, IRemoteWorker worker)
        {
            var list = workers.GetOrAdd(key, new ConcurrentBag<IRemoteWorker>());
            list.Add(worker);
        }

        public IList<IRemoteWorker> GetWorkers(string key)
        {
            ConcurrentBag<IRemoteWorker> list;
            if (workers.TryGetValue(key, out list))
            {
                return list.ToList();
            }
            return null;
        }

        public void AddMigration(string key, IRemoteWorker worker, IEnumerable<AbstractSolution> solutions)
        {
            var list = migrations.GetOrAdd(key, new ConcurrentDictionary<IRemoteWorker, IEnumerable<AbstractSolution>>());
            var isAdded = list.TryAdd(worker, solutions);
            Debug.Assert(isAdded);
        }

        public IDictionary<IRemoteWorker, IEnumerable<AbstractSolution>> GetMigrations(string key)
        {
            ConcurrentDictionary<IRemoteWorker, IEnumerable<AbstractSolution>> list;
            if (migrations.TryGetValue(key, out list))
            {
                return list.ToDictionary(item => item.Key, item => item.Value);
            }
            return null;
        }

        public void ResetMigrations(string key)
        {
            ConcurrentDictionary<IRemoteWorker, IEnumerable<AbstractSolution>> list;
            if (migrations.TryGetValue(key, out list))
            {
                list.Clear();
            }
        }

        public async Task UpdateResultAsync(string key, AbstractSolution bestSolution)
        {
            var task = tasks[key];
            var partitionKey = task.Problem.GetType().Name;

            var result = await GetResultAsync(partitionKey, key);
            if (result == null)
            {
                result = new Entities.Result(partitionKey, key)
                {
                    BestQuality = bestSolution.Quality,
                    NumberOfWorkers = workers[key].Count,
                    RunDurationMilliseconds = (task.EndTime.Value - task.StartTime.Value).TotalMilliseconds
                };
            }
            else
            {
                result.BestQuality = task.Problem.Maximization ? Math.Max(result.BestQuality, bestSolution.Quality) : Math.Min(result.BestQuality, bestSolution.Quality);
            }

            var table = await GetOrCreateResultsTableAsync();
            await table.ExecuteAsync(TableOperation.InsertOrMerge(result));
        }

        public async Task<DataContracts.Result> GetResultAsync(string key)
        {
            var task = tasks[key];
            var partitionKey = task.Problem.GetType().Name;

            var result = await GetResultAsync(partitionKey, key);
            return new DataContracts.Result(TimeSpan.FromMilliseconds(result.RunDurationMilliseconds), result.BestQuality, result.NumberOfWorkers);
        }

        private async Task<Entities.Result> GetResultAsync(string partitionKey, string rowKey)
        {
            var table = await GetOrCreateResultsTableAsync();
            return (Entities.Result)(await table.ExecuteAsync(TableOperation.Retrieve<Entities.Result>(partitionKey, rowKey))).Result;
        }

        private async Task<CloudTable> GetOrCreateResultsTableAsync()
        {
            var table = GetResultsTable();
            await table.CreateIfNotExistsAsync();
            return table;
        }

        private CloudTable GetResultsTable()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("results");
        }

        public async Task ClearResultsAsync()
        {
            var table = GetResultsTable();
            var results = table.ExecuteQuery<Entities.Result>(new TableQuery<Entities.Result>());

            if (results.Any())
            {
                var operation = new TableBatchOperation();
                foreach (var result in results)
                {
                    operation.Add(TableOperation.Delete(result));
                }
                await table.ExecuteBatchAsync(operation);
            }

            tasks.Clear();
            workers.Clear();
        }
    }
}
