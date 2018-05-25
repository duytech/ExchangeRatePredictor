using ExchangeRatePredictor.Foundation;
using ExchangeRatePredictor.Foundation.Application;
using System;
using System.IO;
using System.Reflection;

namespace ExchangeRatePredictor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Console.WriteLine("Hello World!");

                IApplication app = new CmdApplication();
                string result = app.Process(args);

                //Predictor predictor = new Predictor("d9019abeb1294959af6be5c240556bfd", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\ExchangeData", "USD", "VND");
                //decimal rate = predictor.Predict(DateTime.Parse("15/01/2016"), DateTime.Parse("15/12/2016"), DateTime.Parse("15/01/2017"));

                Console.WriteLine(result);
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
