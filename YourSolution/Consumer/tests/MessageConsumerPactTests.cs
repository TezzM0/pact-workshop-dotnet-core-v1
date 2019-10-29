using System;
using System.Collections.Generic;
using System.Text;
using PactNetMessages;
using Xunit;

namespace tests
{
    public class MessageConsumerPactTests : IClassFixture<MessageConsumerPactClassFixture>
    {
        private MessageConsumerPactClassFixture _fixture;

        public MessageConsumerPactTests(MessageConsumerPactClassFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ItHasProducerInTheName()
        {
            _fixture.PactMessageBuilder.MockMq()
                .Given("There is data")
                .UponReceiving("A message")
                .WithMetaData(new { })
                .WithContent(new {Name = "Provider"});
        }
    }
}
