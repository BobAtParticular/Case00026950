using System;
using Case00026950.Messages.SomeBusinessProcess.Commands;
using NServiceBus;

namespace Case00026950
{
    public static class SharedEndpointConfiguration
    {
        public const string NsbSqlConnection = @"Data Source=.\SqlExpress;Database=NServiceBus;Integrated Security=True";

        public static EndpointConfiguration CreateEndpointConfiguration(string endpointName, Action<RoutingSettings<SqlServerTransport>> customRouting = null)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(NsbSqlConnection);

            var routing = transport.Routing();

            routing.RouteToEndpoint(typeof(StartSomeBusinessProcess).Assembly, "Case00026950.Handler");

            customRouting?.Invoke(routing);

            endpointConfiguration.SendFailedMessagesTo("error");

            endpointConfiguration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && (t.Namespace.EndsWith(".Commands")))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith(".Events"));

            return endpointConfiguration;
        }
    }
}
