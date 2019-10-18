using System.Collections.Generic;
using Consumer;
using Xunit;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using System;
using System.Web;

namespace tests
{
    public class ConsumerPactTests : IClassFixture<ConsumerPactClassFixture>
    {
        private IMockProviderService _mockProviderService;
        private string _mockProviderServiceBaseUri;

        public ConsumerPactTests(ConsumerPactClassFixture fixture)
        {
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions(); //NOTE: Clears any previously registered interactions before the test is run
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public void ItHandlesInvalidDateParam()
        {
            // Arange
            var invalidRequestMessage = "validDateTime is not a date or time";
            _mockProviderService.Given("There is data")
                                .UponReceiving("A invalid GET request for Date Validation with invalid date parameter")
                                .With(new ProviderServiceRequest 
                                {
                                    Method = HttpVerb.Get,
                                    Path = "/api/provider",
                                    Query = "validDateTime=lolz"
                                })
                                .WillRespondWith(new ProviderServiceResponse {
                                    Status = 400,
                                    Headers = new Dictionary<string, object>
                                    {
                                        { "Content-Type", "application/json; charset=utf-8" }
                                    },
                                    Body = new 
                                    {
                                        message = invalidRequestMessage
                                    }
                                });

             // Act
            var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("lolz", _mockProviderServiceBaseUri).GetAwaiter().GetResult();
            var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            // Assert
            Assert.Contains(invalidRequestMessage, resultBodyText);
        }

        [Fact]
        public void ItHandlesValidDateParam()
        {
            // Arange
            var testDateTime = new DateTime(2010, 11, 12, 10, 13, 45);
            var expectedDateTime = "12-11-2010+10%3a13%3a45";
            _mockProviderService.Given("There is data")
                                .UponReceiving("A valid GET request for Date Validation with valid date parameter")
                                .With(new ProviderServiceRequest 
                                {
                                    Method = HttpVerb.Get,
                                    Path = "/api/provider",
                                    Query = "validDateTime=12/11/2010%2010:13:45"
                                })
                                .WillRespondWith(new ProviderServiceResponse {
                                    Status = 400,
                                    Headers = new Dictionary<string, object>
                                    {
                                        { "Content-Type", "application/json; charset=utf-8" }
                                    },
                                    Body = new 
                                    {
                                        message = new {
                                            test = "NO",
                                            validDateTime = expectedDateTime
                                        }
                                    }
                                });

             // Act
            var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("12/11/2010 10:13:45", _mockProviderServiceBaseUri).GetAwaiter().GetResult();
            var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            // Assert
            Assert.Contains(expectedDateTime, resultBodyText);
        }
    }
}
