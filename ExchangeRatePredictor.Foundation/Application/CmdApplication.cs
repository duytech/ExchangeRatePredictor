using CommandLine;
using ExchangeRatePredictor.Foundation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation.Application
{
    public class CmdApplication : IApplication
    {
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
            Predictor predictor = new Predictor("d9019abeb1294959af6be5c240556bfd", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\ExchangeData", opts.From, opts.To);
            decimal rate = predictor.Predict(DateTime.Parse("15/01/2016"), DateTime.Parse("15/12/2016"), DateTime.Parse("15/01/2017"));

            return rate.ToString();
        }

        private string ShowError(IEnumerable<Error> errors)
        {
            throw new NotImplementedException();
        }
    }
}
