using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExchangeRatePredictor.Foundation
{
    /// <summary>
    /// Calculate the prediction of the exchange rate for 15/1/2017 using 12 sample points, one per month, from
    /// the 15th of the month for the period 15/1/2016 through 15/12/2016
    /// </summary>
    public class Predictor
    {
        private readonly string _appId;
        private readonly string _path;
        private readonly string fromCurrency;
        private readonly string toCurrency;

        public Predictor(string appId, string path, string fromCurrency, string toCurrency)
        {
            _appId = appId;
            _path = path;
            this.fromCurrency = fromCurrency;
            this.toCurrency = toCurrency;
        }

        /// <summary>
        /// Ordinal month value
        /// </summary>
        /// <returns></returns>
        private int[] GetX(DateTime fromDate, DateTime toDate)
        {
            return Enumerable.Range(fromDate.Month, toDate.Month).ToArray();
        }

        private decimal[] GetY(DateTime fromDateInput, DateTime toDateInput, int[] xValues)
        {
            IList<decimal> list = new List<decimal>();
            var reader = new ExchangeRateDataReader(_path, _appId);
            foreach(int x in xValues)
            {
                var date = GetDateForMonth(fromDateInput, toDateInput, x);
                //var rate = GetExchangeRate(date, appId);
                var rate = reader.Read(date, fromCurrency);
                var y = Decimal.Parse(rate.rates[toCurrency].ToString());

                list.Add(y);
            }

            return list.ToArray();
        }

        public IDictionary CreateXYTable(int[] xValues, decimal[] yValues)
        {
            IDictionary dictionary = new Dictionary<int, decimal>();
            for (int i = 0; i < xValues.Length; i++)
            {
                dictionary.Add(xValues[i], yValues[i]);
            }

            return dictionary;
        }

        public decimal Predict(DateTime fromDateInput, DateTime toDateInput, DateTime dateToPredict)
        {
            int[] xValues = GetX(fromDateInput, toDateInput);
            decimal[] yValues = GetY(fromDateInput, toDateInput, xValues);

            // step 0
            var table = CreateXYTable(xValues, yValues);

            // step 1
            // number of values
            int n = table.Count;

            // step 2
            decimal[] xx = CalXX(xValues);
            decimal[] xy = CalXY(xValues, yValues);

            // step 3
            int sumX = xValues.Sum();
            decimal sumY = yValues.Sum();
            decimal sumXX = xx.Sum();
            decimal sumXY = xy.Sum();

            // step 4
            decimal slope = CalSlope(n, sumX, sumY, sumXX, sumXY);

            // step 5
            decimal intercept = CalIntercept(sumY, slope, sumX, n);

            // step 6
            // 15/1/2017 => x = 13
            decimal rate = CalRate(intercept, slope, 13);

            return rate;
        }

        /// <summary>
        /// Slope(b) = (NΣXY - (ΣX)(ΣY)) / (NΣX2 - (ΣX)2)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="sumX"></param>
        /// <param name="sumY"></param>
        /// <param name="sumXX"></param>
        /// <param name="sumXY"></param>
        /// <returns></returns>
        private decimal CalSlope(int n, int sumX, decimal sumY, decimal sumXX, decimal sumXY)
        {
            return (n * sumXY - sumX * sumY) / (n * sumXX - (decimal)Math.Pow(sumX, 2));
        }

        /// <summary>
        /// Intercept(a) = (ΣY - b(ΣX)) / N 
        /// </summary>
        /// <returns></returns>
        private decimal CalIntercept(decimal sumY, decimal b, decimal sumX, int n)
        {
            return (sumY - b * sumX) / n;
        }

        private decimal[] CalXX(int[] xValues)
        {
            IList<decimal> list = new List<decimal>();
            foreach (var x in xValues)
            {
                list.Add(x * x);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Regression Equation(y) = a + bx 
        /// </summary>
        /// <returns></returns>
        private decimal CalRate(decimal intercept, decimal slope, int x)
        {
            return intercept + slope * x;
        }

        private decimal[] CalXY(int[] xValues, decimal[] yValues)
        {
            IList<decimal> list = new List<decimal>();
            for (int i=0; i<xValues.Length; i++)
            {
                list.Add(xValues[i] * yValues[i]);
            }

            return list.ToArray();
        }

        private int SumOfX(int[] xValues)
        {
            return xValues.Sum();
        }

        private decimal SumOfY(decimal[] yValues)
        {
            return yValues.Sum();
        }

        private DateTime GetDateForMonth(DateTime fromDateInput, DateTime toDateInput, int month)
        {
            var date = new DateTime();
            var isParsed = DateTime.TryParseExact($"{fromDateInput.Day}/{month}/{fromDateInput.Year}", new string[] { "dd/M/yyyy", "dd/MM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            if (!isParsed)
                throw new ArgumentException("Date is not valid. Date format is dd/M/yyyy or dd/MM/yyyy");

            return date;
        }

        private ExchangeRate GetExchangeRate(DateTime exchangeRateDate, string appId)
        {
            var client = new OpenExchangeRateClient();

            var exchangeRate = client.GetRateByDate(exchangeRateDate, appId, null);

            return exchangeRate;
        }
    }
}
