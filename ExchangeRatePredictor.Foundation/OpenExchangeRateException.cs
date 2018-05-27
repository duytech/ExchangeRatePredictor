using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation
{
    public class OpenExchangeRateException : Exception
    {
        public OpenExchangeRateException()
        {
        }

        public OpenExchangeRateException(string message)
            : base(message)
        {
        }

        public OpenExchangeRateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
