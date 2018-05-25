using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation.Models
{
    [Verb("predict", HelpText = "Get information from outside service.")]
    public class PredictOption
    {
        [Option('f', "from", Required = true, HelpText = "From currency to be processed.")]
        public string From { get; set; }

        [Option('t', "to", Required = true, HelpText = "To currency to be processed.")]
        public string To { get; set; }
    }
}
