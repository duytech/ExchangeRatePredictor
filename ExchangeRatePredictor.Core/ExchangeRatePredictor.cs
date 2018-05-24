using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Core
{
    public class ExchangeRatePredictor
    {
        private double[] GetX()
        {
            throw new NotImplementedException();
        }

        private async Task<ExchangeRate> GetRateByDate(DateTime date, string appId)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync($"https://openexchangerates.org/api/historical/{date.ToString("yyyy-mm-dd")}.json?app_id={appId}");

            return JsonConvert.DeserializeObject<ExchangeRate>(result);
        }
    }
}
