using System;
using System.Threading.Tasks;
using NServiceBus;

namespace message_consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "message consumer";

            var endpointConfiguration = new EndpointConfiguration("message consumer");

            endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
