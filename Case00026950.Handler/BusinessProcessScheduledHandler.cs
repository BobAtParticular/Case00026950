using System.Threading.Tasks;
using Case00026950.Messages.SomeBusinessProcess.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Case00026950.Handler
{
    public sealed class BusinessProcessScheduledHandler : IHandleMessages<BusinessProcessScheduled>
    {
        private static readonly ILog Log = LogManager.GetLogger("Case00026950Handler");

        public Task Handle(BusinessProcessScheduled message, IMessageHandlerContext context)
        {
            Log.InfoFormat("BusinessProcessScheduled event received for transaction = {0}", message.TransactionId);

            return Task.CompletedTask;
        }
    }
}
