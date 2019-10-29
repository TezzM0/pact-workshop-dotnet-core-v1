using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using shared;

namespace message_consumer
{
    public class TestMessageHandler : IHandleMessages<TestMessage>
    {
        public Task Handle(TestMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received {message.Name} - sequence: {message.SequenceNumber} for timestamp: {message.Timestamp}");
            return Task.CompletedTask;
        }
    }
}
