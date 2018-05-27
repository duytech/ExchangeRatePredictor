using ExchangeRatePredictor.Foundation;
using ExchangeRatePredictor.Foundation.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeRatePredictor.Tests.IntegrationTests
{
    public class IntegrationTest
    {
        [Fact]
        public void TestPredictExchangeRateForSuccess()
        {
            string command = "predict -f USD -t VND";
            string[] args = command.Split(' ');
            int exitCode = Program.Main(args);
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void TestPredictExchangeRateForFail()
        {
            string command = "predict -f USD1 -t VND";
            string[] args = command.Split(' ');
            int exitCode = Program.Main(args);
            Assert.Equal(0, exitCode);
        }
    }
}
