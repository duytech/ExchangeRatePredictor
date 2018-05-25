using ExchangeRatePredictor.Foundation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation.Handlers
{
    public interface IPredictCmdHandler
    {
        bool Handle(PredictOption option);
    }
}
