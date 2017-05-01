using System.Threading.Tasks;
using Case00026950.Messages.SomeBusinessProcess.Commands;
using Case00026950.Messages.SomeBusinessProcess.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Case00026950.Handler
{
    public sealed class StartSomeBusinessProcessHandler : IHandleMessages<StartSomeBusinessProcess>
    {
        private static readonly ILog Log = LogManager.GetLogger("Case00026950Handler");

        public async Task Handle(StartSomeBusinessProcess message, IMessageHandlerContext context)
        {
            Log.InfoFormat("Starting StartSomeBusinessProcess workflow for transaction = {0}", message.TransactionId);

            await Task.Delay(1000);

            Log.InfoFormat("publishing BusinessProcessScheduled for transaction = {0}", message.TransactionId);
            await context.Publish(new BusinessProcessScheduled { TransactionId = message.TransactionId });
        }
    }
}
