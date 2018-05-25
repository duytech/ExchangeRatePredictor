using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation.Application
{
    public interface IApplication
    {
        string Process(string[] args);
    }
}
