using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Diga.Domain.Service
{
    public class StateManager
    {
        private static Lazy<StateManager> instance = new Lazy<StateManager>(() => new StateManager());

        private ConcurrentDictionary<string, DataContracts.OptimizationTask> tasks =
            new ConcurrentDictionary<string, DataContracts.OptimizationTask>();

        private ConcurrentDictionary<string, ConcurrentBag<IDigaCallback>> workers =
            new ConcurrentDictionary<string, ConcurrentBag<IDigaCallback>>();

        public static StateManager Instance
        {
            get { return instance.Value; }
        }

        public bool AddTask(string key, DataContracts.OptimizationTask task)
        {
            task.StartTime = DateTime.MinValue;
            return tasks.TryAdd(key, task);
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

        public DataContracts.OptimizationTask RemoveTask(string key)
        {
            DataContracts.OptimizationTask task;
            if (tasks.TryRemove(key, out task))
            {
                return task;
            }
            return null;
        }

        public void AddWorker(string key, IDigaCallback worker)
        {
            var list = workers.GetOrAdd(key, new ConcurrentBag<IDigaCallback>());
            list.Add(worker);
        }

        public async Task UpdateResultAsync(string taskKey, AbstractSolution bestSolution)
        {
            var task = tasks[taskKey];
            var partitionKey = task.Problem.GetType().Name;

            var result = await GetResultAsync(partitionKey, taskKey);
            if (result == null)
            {
                result = new Entities.Result(partitionKey, taskKey)
                {
                    BestQuality = bestSolution.Quality,
                    NumberOfWorkers = workers[taskKey].Count,
                    RunDurationMilliseconds = (DateTime.Now - task.StartTime).TotalMilliseconds
                };
            }
            else
            {
                result.BestQuality = task.Problem.Maximization ? Math.Max(result.BestQuality, bestSolution.Quality) : Math.Min(result.BestQuality, bestSolution.Quality);
            }

            var table = await GetOrCreateResultsTableAsync();
            await table.ExecuteAsync(TableOperation.InsertOrMerge(result));
        }

        public async Task<DataContracts.Result> GetResultAsync(string taskKey)
        {
            var task = tasks[taskKey];
            var partitionKey = task.Problem.GetType().Name;

            var result = await GetResultAsync(partitionKey, taskKey);
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
        }
    }
}
