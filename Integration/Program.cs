using Integration.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integration;

public abstract class Program
{

    public static void Main(string[] args)
    {
        // Build configuration
        var config = new ConfigurationBuilder()
           .SetBasePath(Environment.CurrentDirectory)
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .Build();

        // Save DI container
        var services = new ServiceCollection();
        services.AddIntegrationServices(config);

        // create service provider
        var serviceProvider = services.BuildServiceProvider();

        // get service from DI container
        var service = serviceProvider.GetRequiredService<ItemIntegrationService>();

        string[] items = new[] { "a", "b", "c" };


        //Test 1
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem(items[0]));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem(items[1]));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem(items[2]));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem(items[0]));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem(items[1]));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem(items[2]));

        Thread.Sleep(500);



        //Test 2

        int[] randomNumberArray = new[] { 1, 2, 3, 1, 2, 3 };

        Parallel.ForEach(randomNumberArray, number =>
        {
            service.SaveItem(number.ToString());
        });

        Thread.Sleep(5000);

        Console.WriteLine("All items: " + string.Join(", ", items) + ", " + string.Join(", ", randomNumberArray.Select(x => x.ToString())));

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}