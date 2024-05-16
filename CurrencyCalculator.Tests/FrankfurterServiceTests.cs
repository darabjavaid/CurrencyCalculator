using CurrencyCalculator.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyCalculator.Tests
{
        public class FrankfurterServiceTests
    {
        private readonly IFrankfurterService _frankfurterService;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

        public FrankfurterServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            //var client = new HttpClient(new MockHttpMessageHandler()) { BaseAddress = new Uri("https://api.frankfurter.app") };
            var client = new HttpClient();
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            _frankfurterService = new FrankfurterService(_mockHttpClientFactory.Object);
        }

        [Fact]
        public async Task GetLatestRatesAsync_ShouldReturnRates()
        {
            var result = await _frankfurterService.GetLatestRatesAsync("EUR");
            Assert.NotNull(result);
            Assert.Equal("EUR", result.Base);
        }

        [Fact]
        public async Task GetLatestRatesAsync_ShouldReturnRatesForBaseCurrencyUSD()
        {
            var result = await _frankfurterService.GetLatestRatesAsync("USD");
            Assert.NotNull(result);
            Assert.Equal("USD", result.Base);
        }

        [Fact]
        public async Task ConvertCurrencyAsync_ShouldExcludeCertainCurrencies()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _frankfurterService.ConvertCurrencyAsync("EUR", 100, "TRY"));
            Assert.Equal("Currency conversion not supported for the specified target currency.", exception.Message);
        }

        [Fact]
        public async Task ConvertCurrencyAsync_ShouldReturnCurrencyRatesForbaseAUDtoINRWithAmount10()
        {
            var result = await _frankfurterService.ConvertCurrencyAsync("AUD", 10, "INR");
            Assert.NotNull(result);
            Assert.Equal("AUD", result.Base);
            Assert.Equal("INR", result.Rates.FirstOrDefault().Key);
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_ShouldReturnPaginatedHistoricalRates()
        {
            var result = await _frankfurterService.GetHistoricalRatesAsync("EUR", new DateTime(2020, 01, 01), new DateTime(2020, 01, 31), 1, 5);
            Assert.NotNull(result);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(5, result.PageSize);
            Assert.Single(result.Items); // Since it's a dictionary with paginated data inside
        }

        //The test case verifies that pagination is correctly implemented.
        [Fact]
        public async Task GetHistoricalRatesAsync_ShouldHaveCorrectPagination()
        {
            var result = await _frankfurterService.GetHistoricalRatesAsync("EUR", new DateTime(2020, 01, 01), new DateTime(2020, 01, 31), 1, 6);
            Assert.NotNull(result);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(6, result.PageSize);
            Assert.True(result.Items.Count <= 6);
        }

        private class MockHttpMessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"base\":\"EUR\",\"date\":\"2024-05-16\",\"rates\":{\"AUD\":1.629,\"BGN\":1.9558,\"BRL\":5.5696,\"CAD\":1.481,\"CHF\":0.9822,\"CNY\":7.843,\"CZK\":24.709,\"DKK\":7.4612,\"GBP\":0.8585,\"HKD\":8.4798,\"HUF\":386.18,\"IDR\":17297,\"ILS\":4.0057,\"INR\":90.73,\"ISK\":150.3,\"JPY\":168.33,\"KRW\":1463.04,\"MXN\":18.1784,\"MYR\":5.0885,\"NOK\":11.642,\"NZD\":1.7794,\"PHP\":62.49,\"PLN\":4.2648,\"RON\":4.9755,\"SEK\":11.6172,\"SGD\":1.4617,\"THB\":39.291,\"TRY\":34.99,\"USD\":1.0866,\"ZAR\":19.7911}}")
                };
                return Task.FromResult(response);
            }
        }
    }

}
