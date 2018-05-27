using ExchangeRatePredictor.Foundation.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace ExchangeRatePredictor.Foundation
{
    public class OpenExchangeRateClient : IOpenExchangeRateClient
    {
        private HttpClient _httpClient;

        public OpenExchangeRateClient()
        {
            _httpClient = new HttpClient();
        }

        public string GetCurrencies()
        {
            string url = "https://openexchangerates.org/api/currencies.json";
            string result = _httpClient.GetStringAsync(url).Result;

            return result;
        }

        public string GetRateByDate(DateTime date, string appId, string baseCurrency)
        {
            string url = $"https://openexchangerates.org/api/historical/{date.ToString("yyyy-MM-dd")}.json?app_id={appId}&base={(string.IsNullOrEmpty(baseCurrency) ? "USD" : baseCurrency)}";
            HttpResponseMessage result = _httpClient.GetAsync(url).Result;
            if (!result.IsSuccessStatusCode)
            {
                string message = result.Content.ReadAsStringAsync().Result;
                JObject errorObject = JObject.Parse(message);
                throw new OpenExchangeRateException(errorObject["description"].ToString());
            }

            return result.Content.ReadAsStringAsync().Result;
        }
    }
}
