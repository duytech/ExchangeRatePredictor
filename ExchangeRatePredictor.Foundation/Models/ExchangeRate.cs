using Newtonsoft.Json.Linq;

namespace ExchangeRatePredictor.Foundation.Models
{
    public class ExchangeRate
    {
        public string disclaimer { get; set; }
        public string license { get; set; }
        public int timestamp { get; set; }
        public string _base { get; set; }
        public JObject rates { get; set; }
    }
}
