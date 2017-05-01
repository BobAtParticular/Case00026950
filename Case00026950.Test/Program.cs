using System;
using System.Threading.Tasks;
using Case00026950.Messages.SomeBusinessProcess.Commands;
using NServiceBus;

namespace Case00026950.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Task.WaitAll(MakeItSo());
            }

            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            Console.Write("Finished Hit a Key to exit");
            Console.ReadKey();
        }

        private static async Task MakeItSo()
        {
            var endpointConfiguration = SharedEndpointConfiguration.CreateEndpointConfiguration("Case00026950.Test");

            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            endpointConfiguration.SendOnly();

            var bus = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            var transactionId = Guid.NewGuid();

            Console.Write("Hit any key to send");
            Console.ReadKey();

            await bus.Send(new StartSomeBusinessProcess
            {
                TransactionId = transactionId
            }).ConfigureAwait(false);

            Console.WriteLine($"Sent StartSomeBusinessProcess for transaction = {transactionId}");

            await bus.Stop().ConfigureAwait(false);
        }
    }
}
