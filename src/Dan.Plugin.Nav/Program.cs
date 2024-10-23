using Dan.Plugin.Nav;
using Microsoft.Extensions.Hosting;
using Dan.Common.Extensions;
using Dan.Plugin.Nav.Clients;
using Dan.Plugin.Nav.Mappers;
using Dan.Plugin.Nav.Models;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureDanPluginDefaults()
    .ConfigureAppConfiguration((context, configuration) =>
    {
        // Add more configuration sources if necessary. ConfigureDanPluginDefaults will load environment variables, which includes
        // local.settings.json (if developing locally) and applications settings for the Azure Function
    })
    .ConfigureServices((context, services) =>
    {
        // Add any additional services here
        services.AddTransient<INavClient, NavClient>();
        services.AddTransient<IEmploymentHistoryMapper, EmploymentHistoryMapper>();

        // This makes IOption<Settings> available in the DI container.
        var configurationRoot = context.Configuration;
        services.Configure<Settings>(configurationRoot);
    })
    .Build();

await host.RunAsync();
