using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNetMessages.Mocks.MockMq.Models;
using provider;
using tests.Middleware;
using tests.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace tests
{
    public class ProviderApiTests : IDisposable
    {
        private string _providerUri { get; }
        private string _pactServiceUri { get; }
        private IWebHost _webHost { get; }
        private ITestOutputHelper _outputHelper { get; }

        public ProviderApiTests(ITestOutputHelper output)
        {
            _outputHelper = output;
            _providerUri = "http://localhost:9000";
            _pactServiceUri = "http://localhost:9001";

            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(_pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        [Fact]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {                
                PublishVerificationResults = true,
                ProviderVersion = "c2f8f069f69dd979039a18b0c7fa5ce161db93b3",

                // NOTE: We default to using a ConsoleOutput,
                // however xUnit 2 does not capture the console output,
                // so a custom outputter is required.
                Outputters = new List<IOutput> { new XUnitOutput(_outputHelper) },

                // Output verbose verification logs to the test output
                Verbose = true
            };

            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier.ProviderState($"{_pactServiceUri}/provider-states")
                .ServiceProvider("Provider", _providerUri)
                .HonoursPactWith("Consumer")
                .PactUri(@"https://test_als_terry.pact.dius.com.au/pacts/provider/Provider/consumer/Consumer/latest", new PactUriOptions("XUxGLRjlW2wM-CBBFK0P9w"))
                .Verify();
        }

        [Fact]
        public void EnsureMessageProviderHonoursPactWithMessageConsumer()
        {

            var config = new PactNetMessages.PactVerifierConfig
            {
                PublishVerificationResults = true,
                ProviderVersion = "c2f8f069f69dd979039a18b0c7fa5ce161db93b3",
            };

            using (var pactVerifier =
                new PactNetMessages.PactVerifier(() => { }, () => { }, config))
            {
                pactVerifier
                    .ProviderState("There is data", setUp: SetUpScenario);

                pactVerifier
                    .MessageProvider("Provider")
                    .HonoursPactWith("message consumer")
                    .PactUri(
                        @"https://test_als_terry.pact.dius.com.au/pacts/provider/Provider/consumer/message%20consumer/latest",
                        new PactNetMessages.PactUriOptions("XUxGLRjlW2wM-CBBFK0P9w"))
                    .Verify();
            }
        }

        private Message SetUpScenario()
        {
            var messageProducer = new MessageProducer();
            ProviderStateHelper.AddData();
            return new Message()
            {
                Contents = messageProducer.ProduceMessage()
            };
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _webHost.StopAsync().GetAwaiter().GetResult();
                    _webHost.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}