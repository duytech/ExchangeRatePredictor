using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace ExchangeRatePredictor.Foundation
{
    public class OpenExchangeRateClient
    {
        private HttpClient _httpClient;

        public OpenExchangeRateClient()
        {
            _httpClient = new HttpClient();
        }

        public ExchangeRate GetRateByDate(DateTime date, string appId, string baseCurrency)
        {
            var result = _httpClient.GetStringAsync($"https://openexchangerates.org/api/historical/{date.ToString("yyyy-MM-dd")}.json?app_id={appId}&base={(string.IsNullOrEmpty(baseCurrency) ? "USD" : baseCurrency)}").Result;

            return JsonConvert.DeserializeObject<ExchangeRate>(result);
        }
    }
}
