using ExchangeRatePredictor.Foundation.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExchangeRatePredictor.Foundation
{
    /// <summary>
    /// Calculate the prediction of the exchange rate for 15/1/2017 using 12 sample points, one per month, from
    /// the 15th of the month for the period 15/1/2016 through 15/12/2016
    /// </summary>
    public class Predictor : IPredictor
    {
        private readonly IExchangeRateDataReader _reader;

        public Predictor(IExchangeRateDataReader reader)
        {
            _reader = reader;
        }

        private IDictionary<int, DateTime> CreateDateSet(DateTime fromDate, DateTime toDate)
        {
            IDictionary<int, DateTime> dateTimes = new Dictionary<int, DateTime>();
            for (int month = fromDate.Month; month <= toDate.Month; month++)
            {
                dateTimes.Add(month, new DateTime(fromDate.Year, month, fromDate.Day));
            }

            return dateTimes;
        }

        private decimal[] GetY(DateTime fromDateInput, DateTime toDateInput, int[] xValues, string fromCurrency, string toCurrency)
        {
            IList<decimal> yValues = new List<decimal>();
            IDictionary<int, DateTime> dates = CreateDateSet(fromDateInput, toDateInput);
            foreach (int xValue in xValues)
            {
                ExchangeRate exhangeRate = _reader.Read(dates[xValue], fromCurrency);
                if (!exhangeRate.rates.ContainsKey(toCurrency))
                    throw new ArgumentException($"Currency code {toCurrency} is invalid.");

                decimal yValue = Decimal.Parse(exhangeRate.rates[toCurrency].ToString());

                yValues.Add(yValue);
            }

            return yValues.ToArray();
        }

        public decimal Predict(PredictorOption option)
        {
            // step 0: create a value table of X(ordinal month value) and Y(rate value for given month)
            int[] xValues = Enumerable.Range(option.FromDate.Month, option.ToDate.Month).ToArray();
            decimal[] yValues = GetY(option.FromDate, option.ToDate, xValues, option.FromCurrency, option.ToCurrency);

            // step 1: number of values
            int n = xValues.Length;

            // step 2: X*X, X*Y
            decimal[] xx = CalXX(xValues);
            decimal[] xy = CalXY(xValues, yValues);

            // step 3: ΣX, ΣY, Σ(X*X) and Σ(X*Y)
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
            decimal rate = CalRate(intercept, slope, xValues.Length + 1);

            return rate;
        }

        /// <summary>
        /// Slope(b) = (NΣXY - (ΣX)(ΣY)) / (NΣX2 - (ΣX)2)
        /// </summary>
        private decimal CalSlope(int n, int sumX, decimal sumY, decimal sumXX, decimal sumXY)
        {
            return (n * sumXY - sumX * sumY) / (n * sumXX - (decimal)Math.Pow(sumX, 2));
        }

        /// <summary>
        /// Intercept(a) = (ΣY - b(ΣX)) / N 
        /// </summary>
        private decimal CalIntercept(decimal sumY, decimal b, decimal sumX, int n)
        {
            return (sumY - b * sumX) / n;
        }

        private decimal[] CalXX(int[] xValues)
        {
            decimal[] xx = new decimal[xValues.Length];
            for (int i = 0; i < xValues.Length; i++)
            {
                xx[i] = (xValues[i] * xValues[i]);
            }

            return xx;
        }

        /// <summary>
        /// Regression Equation(y) = a + bx 
        /// </summary>
        private decimal CalRate(decimal intercept, decimal slope, int x)
        {
            return intercept + slope * x;
        }

        private decimal[] CalXY(int[] xValues, decimal[] yValues)
        {
            decimal[] xy = new decimal[xValues.Length];
            for (int i = 0; i < xValues.Length; i++)
            {
                xy[i] = xValues[i] * yValues[i];
            }

            return xy;
        }
    }
}
