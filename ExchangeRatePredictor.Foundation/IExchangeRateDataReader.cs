using ExchangeRatePredictor.Foundation.Models;
using Newtonsoft.Json.Linq;
using System;

namespace ExchangeRatePredictor.Foundation
{
    public interface IExchangeRateDataReader
    {
        ExchangeRate Read(DateTime date, string baseCurrency);
        JObject ReadCurrencies();
    }
}
