using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation
{
    public class ExchangeRateDataReader
    {
        private readonly string path;
        private readonly string appId;

        public ExchangeRateDataReader(string path, string appId)
        {
            this.path = path;
            this.appId = appId;
        }

        private string ReadFile(string fileName)
        {
            Directory.CreateDirectory(path);
            string readText = string.Empty;

            try
            {
                // Open the file to read from.
                readText = File.ReadAllText(path + @"\" + fileName);
            }
            catch(Exception ex)
            {
                return string.Empty;
            }

            return readText;
        }

        public ExchangeRate Read(DateTime date, string baseCurrency)
        {
            var fileName = $"{date.ToString("yyyy-MM-dd")}.json";
            var readText = ReadFile(fileName);

            if (string.IsNullOrEmpty(readText))
            {
                var client = new OpenExchangeRateClient();
                ExchangeRate exchangeRate = client.GetRateByDate(date, appId, baseCurrency);
                readText = JsonConvert.SerializeObject(exchangeRate);

                WriteFile(fileName, readText);
            }

            return JsonConvert.DeserializeObject<ExchangeRate>(readText);
        }

        private void WriteFile(string fileName, string content)
        {
            Directory.CreateDirectory(path);

            // Open the file to read from.
            File.WriteAllText(path + @"\" + fileName, content);
        }
    }
}
