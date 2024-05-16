using CurrencyCalculator.Models;
using Newtonsoft.Json;

namespace CurrencyCalculator.Services
{
    // Services/FrankfurterService.cs
    public interface IFrankfurterService
    {
        Task<ExchangeRatesResponse> GetLatestRatesAsync(string baseCurrency);
        Task<ExchangeRatesResponse> ConvertCurrencyAsync(string baseCurrency, decimal amount, string targetCurrency);
        Task<HistoricalRatesResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate);
        Task<PaginatedResponse<Dictionary<string, Dictionary<string, decimal>>>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    }

    public class FrankfurterService : IFrankfurterService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string BaseUrl = "https://api.frankfurter.app";
        private static readonly string[] ExcludedCurrencies = { "TRY", "PLN", "THB", "MXN" };

        public FrankfurterService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<T> GetAsync<T>(string endpoint, int maxRetries = 3)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < maxRetries; i++)
            {
                response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    break;
                }
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<ExchangeRatesResponse> GetLatestRatesAsync(string baseCurrency)
        {
            return await GetAsync<ExchangeRatesResponse>($"{BaseUrl}/latest?from={baseCurrency}");
        }

        public async Task<ExchangeRatesResponse> ConvertCurrencyAsync(string baseCurrency, decimal amount, string targetCurrency)
        {
            if (ExcludedCurrencies.Contains(baseCurrency) || ExcludedCurrencies.Contains(targetCurrency))
            {
                throw new InvalidOperationException("Currency conversion not supported for the specified target currency.");
            }

            var result = await GetAsync<ExchangeRatesResponse>($"{BaseUrl}/latest?amount={amount}&from={baseCurrency}&to={targetCurrency}");
            result.Rates = result.Rates.Where(r => !ExcludedCurrencies.Contains(r.Key)).ToDictionary(r => r.Key, r => r.Value);

            return result;
        }

        public async Task<HistoricalRatesResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate)
        {
            string startDateString = startDate.ToString("yyyy-MM-dd");
            string endDateString = endDate.ToString("yyyy-MM-dd");
            return await GetAsync<HistoricalRatesResponse>($"{BaseUrl}/{startDateString}..{endDateString}?from={baseCurrency}");
        }

        public async Task<PaginatedResponse<Dictionary<string, Dictionary<string, decimal>>>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            string startDateString = startDate.ToString("yyyy-MM-dd");
            string endDateString = endDate.ToString("yyyy-MM-dd");
            var result = await GetAsync<HistoricalRatesResponse>($"{BaseUrl}/{startDateString}..{endDateString}?base={baseCurrency}");

            // ordering the rates by date
            var allRates = result.Rates.OrderBy(r => r.Key).ToList();

            // Paginating the result
            var paginatedRates = allRates.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Transforming paginatedRates to match the desired type
            var paginatedRatesDictionary = paginatedRates.ToDictionary(r => r.Key, r => r.Value);

            return new PaginatedResponse<Dictionary<string, Dictionary<string, decimal>>>
            {
                Base = result?.Base,
                TotalItems = allRates.Count,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = new List<Dictionary<string, Dictionary<string, decimal>>> { paginatedRatesDictionary }
            };
        }


    }

}
