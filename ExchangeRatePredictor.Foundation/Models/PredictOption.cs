using CommandLine;

namespace ExchangeRatePredictor.Foundation.Models
{
    [Verb("predict", HelpText = "Predict exchange rate by using openexchangerates.org data.")]
    public class PredictOption
    {
        [Option('f', "from", Required = true, HelpText = "The currency will be converted from.")]
        public string From { get; set; }

        [Option('t', "to", Required = true, HelpText = "The currency will be converted to.")]
        public string To { get; set; }
    }
}
