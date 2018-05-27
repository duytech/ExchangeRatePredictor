using ExchangeRatePredictor.Foundation.Models;
using Newtonsoft.Json.Linq;
using System;

namespace ExchangeRatePredictor.Foundation
{
    public interface IOpenExchangeRateClient
    {
        string GetRateByDate(DateTime date, string appId, string baseCurrency);
        string GetCurrencies();
    }
}
