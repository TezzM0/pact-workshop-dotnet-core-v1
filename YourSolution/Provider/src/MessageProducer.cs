using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using shared;

namespace provider
{
    public class MessageProducer
    {
        private IEndpointInstance _endpointInstance;
        private Data _data;

        public MessageProducer()
        {
            _data = new Data();
        }

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

        public async Task SendMessagesIfDataExists()
        {
            var count = 0;
            while (true)
            {
                if (_data.DataIsMissing())
                {
                    break;
                }

                count++;
                Thread.Sleep(1000);
                await _endpointInstance.Send(new TestMessage()
                {
                    Name = "Producer",
                    SequenceNumber = count,
                    Timestamp = DateTime.Now
                }).ConfigureAwait(false);
            }
        }
    }
}
