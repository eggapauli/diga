using Microsoft.WindowsAzure.Storage.Table;

namespace Diga.Domain.Service.Entities
{
    public class Result : TableEntity
    {
        public Result(string problemName, string taskKey)
        {
            this.PartitionKey = problemName;
            this.RowKey = taskKey;
        }

        public Result()
        {
        }

        public double RunDurationMilliseconds { get; set; }

        public double BestQuality { get; set; }

        public int NumberOfWorkers { get; set; }
    }
}
