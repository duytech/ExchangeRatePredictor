using CommandLine;
using ExchangeRatePredictor.Foundation.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ExchangeRatePredictor.Foundation.Application
{
    public class CmdApplication : IApplication
    {
        private readonly IPredictor _predictor;
        private readonly IExchangeRateDataReader _reader;

        public CmdApplication(IPredictor predictor, IExchangeRateDataReader reader)
        {
            _predictor = predictor;
            _reader = reader;
        }

        public string Process(string[] args)
        {
            ParserResult<PredictOption> parsedResult = Parser.Default.ParseArguments<PredictOption>(args);
            string result = parsedResult.MapResult(
                            opts => HandlePredictCommand(opts),
                            errors => ShowError(errors)
                            );

            return result;
        }

        private string HandlePredictCommand(PredictOption opts)
        {
            JObject currencies = _reader.ReadCurrencies();
            StringBuilder stringBuilder = new StringBuilder();
            if (!currencies.ContainsKey(opts.From))
                stringBuilder.AppendLine($"Currency code {opts.From} is invalid.");

            if(!currencies.ContainsKey(opts.To))
                stringBuilder.AppendLine($"Currency code {opts.To} is invalid.");

            if (stringBuilder.Length > 0)
                throw new ArgumentException(stringBuilder.ToString());

            PredictorOption option = new PredictorOption
            {
                FromDate = DateTime.ParseExact("15/01/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                ToDate = DateTime.ParseExact("15/12/2016", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                PredictDate = DateTime.ParseExact("15/01/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                FromCurrency = opts.From,
                ToCurrency = opts.To
            };

            decimal rate = _predictor.Predict(option);

            string result = $"The predicted currency exchange from {opts.From} to {opts.To} for 15/1/2017 is {rate}";

            return result;
        }

        private string ShowError(IEnumerable<Error> errors)
        {
            return "Somthing went wrong.";
        }
    }
}
