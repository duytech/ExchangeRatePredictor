using ExchangeRatePredictor.Foundation;
using ExchangeRatePredictor.Foundation.Models;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExchangeRatePredictor.Tests
{
    public class UnitTest
    {
        [Fact]
        public void Test1()
        {
            Random randNum = new Random();
            int[] yValues = Enumerable.Repeat(0, 12).Select(i => randNum.Next(1, 30)).ToArray();

            Queue<ExchangeRate> rateQueue = new Queue<ExchangeRate>();
            foreach (int yValue in yValues)
            {
                ExchangeRate r = new ExchangeRate();
                r.rates = new JObject(new JProperty("VND", yValue));

                rateQueue.Enqueue(r);
            }

            Mock<IExchangeRateDataReader> readerMock = new Mock<IExchangeRateDataReader>();
            readerMock.Setup(reader => reader.Read(It.IsAny<DateTime>(), It.IsAny<string>())).Returns(rateQueue.Dequeue);

            PredictorOption predictorOption = new PredictorOption
            {
                FromCurrency = "USD",
                ToCurrency = "VND",
                FromDate = new DateTime(year: 2016, month: 1, day: 15),
                ToDate = new DateTime(year: 2016, month: 12, day: 15),
                PredictDate = new DateTime(year: 2017, month: 1, day: 15)
            };

            IPredictor predictor = new Predictor(readerMock.Object);
            decimal rate = predictor.Predict(predictorOption);

            Assert.True(rate > 0);
        }
    }
}
