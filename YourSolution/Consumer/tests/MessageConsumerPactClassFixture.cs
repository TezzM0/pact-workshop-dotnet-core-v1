using System;
using System.Collections.Generic;
using System.Text;
using PactNetMessages;

namespace tests
{
    public class MessageConsumerPactClassFixture : IDisposable
    {
        public IPactMessageBuilder PactMessageBuilder { get; set; }

        public MessageConsumerPactClassFixture()
        {
            var pactConfig = new PactConfig
            {
                PactDir = @"..\..\..\..\..\pacts",
                LogDir = @".\pact_logs"
            };

            PactMessageBuilder = new PactMessageBuilder(pactConfig);

            PactMessageBuilder.ServiceConsumer("message consumer").HasPactWith("Provider");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PactMessageBuilder.Build();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
