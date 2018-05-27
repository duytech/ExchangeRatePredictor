using ExchangeRatePredictor.Foundation;
using ExchangeRatePredictor.Foundation.Application;
using ExchangeRatePredictor.Foundation.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace ExchangeRatePredictor
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ILogger<Program> logger = null;

            try
            {
                IConfiguration configuration = BuildConfiguration();
                IServiceProvider container = ConfigureServices(configuration);
                ConfigureLogger(configuration);

                logger = container.GetService<ILoggerFactory>().CreateLogger<Program>();

                IApplication app = container.GetService<IApplication>();
                string result = app.Process(args);

                logger.LogInformation(result);

                return 1;
            }
            catch (Exception ex)
            {
                try
                {
                    var messages = ex.GetAllMessages();

                    logger.LogError(messages);
                    logger.LogInformation("Please refer to the log file for more information");
                    logger.LogDebug(ex, "Something went wrong.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.ReadLine();
                }

                return 0;
            }
        }

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(config => config.AddSerilog());
            serviceCollection.AddTransient<IOpenExchangeRateClient, OpenExchangeRateClient>();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddTransient<IExchangeRateDataReader, ExchangeRateDataReader>();
            serviceCollection.AddTransient<IPredictor, Predictor>();
            serviceCollection.AddTransient<IApplication, CmdApplication>();

            return serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureLogger(IConfiguration configuration)
        {
            var serilogger = new LoggerConfiguration()
                                    .ReadFrom.Configuration(configuration)
                                    .CreateLogger();

            Log.Logger = serilogger;
        }
    }
}
