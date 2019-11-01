using System.Threading.Tasks;
using NServiceBus;
using shared;

namespace provider
{
    public class MessageSender
    {
        private IEndpointInstance _endpointInstance;

        public MessageSender()
        {
        }

        public MessageProducer MessageProducer { get; set; }

        public async Task SetupNServiceBusAsync()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration("provider");

                var transport = endpointConfiguration.UseTransport<LearningTransport>();

                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(TestMessage), "message consumer");

                _endpointInstance = await Endpoint.Start(endpointConfiguration)
                    .ConfigureAwait(false);
            }
            catch
            {

            }
        }

        public async Task SendMessages()
        {
            var message = MessageProducer.ProduceMessage();
            if (message != null)
            {
                await _endpointInstance.Send(message).ConfigureAwait(false);
            }
        }
    }
}
