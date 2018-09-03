using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PowerStats.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            // create service collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // entry to run app
            serviceProvider.GetService<PowerStatistics>().RunAsync().GetAwaiter().GetResult();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // add logging
            serviceCollection.AddSingleton(new LoggerFactory()
              .AddConsole());
            serviceCollection.AddLogging();

            // add app
            serviceCollection.AddTransient<PowerStatistics>();
        }
    }
}
