using ExchangeRatePredictor.Foundation.Common;
using ExchangeRatePredictor.Foundation.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation
{
    public class ExchangeRateDataReader : IExchangeRateDataReader
    {
        private readonly IConfiguration _configuration;
        private readonly IOpenExchangeRateClient _client;
        private readonly ILogger<ExchangeRateDataReader> _logger;

        public ExchangeRateDataReader(IConfiguration configuration, IOpenExchangeRateClient client, ILogger<ExchangeRateDataReader> logger)
        {
            _configuration = configuration;
            _client = client;
            _logger = logger;
        }

        private string ReadFile(string fileName, string path)
        {
            string readText = string.Empty;

            try
            {
                readText = File.ReadAllText($"{path}\\{fileName}");
            }
            catch(Exception)
            {
                return string.Empty;
            }

            return readText;
        }

        public ExchangeRate Read(DateTime date, string baseCurrency)
        {
            string cachePathBaseCurrency = $"{Environment.ExpandEnvironmentVariables(_configuration[Constants.CacheFolder])}\\{baseCurrency}";
            string fileName = $"{date.ToString("yyyy-MM-dd")}.json";
            string fileContent = ReadFile(fileName, cachePathBaseCurrency);

            if (string.IsNullOrEmpty(fileContent))
            {
                fileContent = _client.GetRateByDate(date, _configuration[Constants.AppId], baseCurrency);

                WriteFile(fileName, fileContent, cachePathBaseCurrency);
            }

            return JsonConvert.DeserializeObject<ExchangeRate>(fileContent);
        }

        private void WriteFile(string fileName, string content, string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                File.WriteAllText($"{path}\\{fileName}", content);
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "Could not cache the downloaded data.");
            }
        }

        public JObject ReadCurrencies()
        {
            string cachePath = Environment.ExpandEnvironmentVariables(_configuration[Constants.CacheFolder]);
            string fileName = "currencies.json";
            string fileContent = ReadFile(fileName, cachePath);

            if (string.IsNullOrEmpty(fileContent))
            {
                fileContent = _client.GetCurrencies();

                WriteFile(fileName, fileContent, cachePath);
            }

            return JObject.Parse(fileContent);
        }
    }
}
