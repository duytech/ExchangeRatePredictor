using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation.Models
{
    public class PredictorOption
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime PredictDate { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
    }
}
