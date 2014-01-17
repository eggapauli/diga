using Diga.Domain.Service.Contracts;
using Diga.Domain.Service.DataContracts;
using Diga.Domain.Service.DataContracts.Solutions;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diga.Domain.Service.Models
{
    public interface IRemoteWorker
    {
        void StartWork(OptimizationTask task);

        void Migrate(IEnumerable<AbstractSolution> solutions);
    }

    public class WcfWorker : IRemoteWorker
    {
        public IDigaCallback CallbackChannel { get; private set; }

        public WcfWorker(IDigaCallback channel)
        {
            CallbackChannel = channel;
        }

        public void StartWork(OptimizationTask task)
        {
            CallbackChannel.StartWork(task);
        }

        public void Migrate(IEnumerable<AbstractSolution> solutions)
        {
            CallbackChannel.Migrate(solutions);
        }
    }

    public class ServiceBusQueueWorker : IRemoteWorker
    {
        private QueueClient queue;

        public Guid Id { get; private set; }

        public ServiceBusQueueWorker(QueueClient queue)
        {
            this.queue = queue;
            Id = Guid.NewGuid();
        }

        public void StartWork(OptimizationTask task)
        {
            var message = new BrokeredMessage(task);
            message.Properties["WorkerId"] = Id;
            queue.Send(message);
        }

        public void Migrate(IEnumerable<AbstractSolution> solutions)
        {
            var message = new BrokeredMessage(solutions);
            message.Properties["WorkerId"] = Id;
            queue.Send(message);
        }
    }

}
