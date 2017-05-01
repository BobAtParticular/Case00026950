using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading.Tasks;
using Case00026950.Messages.SomeBusinessProcess.Events;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using Container = StructureMap.Container;
using IContainer = StructureMap.IContainer;

namespace Case00026950.Handler
{

    [DesignerCategory("Code")]
    class ProgramService : ServiceBase
    {
        private const string EndPointName = "Case00026950.Handler";

        private IEndpointInstance endpoint;
        private IContainer iocContainer;
        private static readonly ILog Logger = LogManager.GetLogger<ProgramService>();

        private static void Main()
        {
            using (var service = new ProgramService())
            {
                // to run interactive from a console or as a windows service
                if (Environment.UserInteractive)
                {
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        service.OnStop();
                    };
                    service.OnStart(null);
                    Console.WriteLine("\r\nPress enter key to stop program\r\n");
                    Console.Read();
                    service.OnStop();
                    return;
                }
                Run(service);
            }
        }

        protected override void OnStart(string[] args)
        {
            AsyncOnStart().GetAwaiter().GetResult();
        }

        async Task AsyncOnStart()
        {
            try
            {
                var defaultFactory = LogManager.Use<DefaultFactory>();
                defaultFactory.Level(LogLevel.Info);

                iocContainer = new Container(new IocContainer())
                {
                    Name = "Root Container"
                };

                var endpointConfiguration = SharedEndpointConfiguration.CreateEndpointConfiguration(EndPointName, routing => routing.RegisterPublisher(typeof(BusinessProcessScheduled), EndPointName));

                endpointConfiguration.UseContainer<StructureMapBuilder>(b => b.ExistingContainer(iocContainer));

                endpointConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(SharedEndpointConfiguration.NsbSqlConnection);

                endpointConfiguration.EnableInstallers();

                endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

                PerformStartupOperations();
            }
            catch (Exception exception)
            {
                Logger.Fatal("Failed to start", exception);
                Environment.FailFast("Failed to start", exception);
            }
        }

        void PerformStartupOperations()
        {
            //TODO: perform any startup operations
        }

        Task OnCriticalError(ICriticalErrorContext context)
        {
            //TODO: Decide if shutting down the process is the best response to a critical error
            // https://docs.particular.net/nservicebus/hosting/critical-errors
            var fatalMessage =
                $"The following critical error was encountered:\n{context.Error}\nProcess is shutting down.";
            Logger.Fatal(fatalMessage, context.Exception);
            Environment.FailFast(fatalMessage, context.Exception);
            return Task.FromResult(0);
        }

        protected override void OnStop()
        {
            endpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}