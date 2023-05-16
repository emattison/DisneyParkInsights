using System;
using Microsoft.Extensions.DependencyInjection;
using DisneyParkInsights;
using Microsoft.Extensions.Configuration;
using Refit;
using DisneyWorldWaitTracker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DisneyParkInsights.DebugHelper;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureLogging(logBuilder =>
    {
        logBuilder.AddConsole();
        logBuilder.AddApplicationInsights();
    })
    .ConfigureServices(serviceCollection =>
    {
        serviceCollection.AddLogging();

#if DEBUG
        serviceCollection.AddSingleton<IThemeParksWiki, DebugThemeParksWiki>();
#else
        serviceCollection.AddRefitClient<IThemeParksWiki>()
                            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.themeparks.wiki"));
#endif

        serviceCollection.AddOptions<AttractionInfoAzureTableStorageConfig>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(AttractionInfoAzureTableStorageConfig)).Bind(settings);
            });

        serviceCollection.AddTransient<IAttractionInfoStorageService, AttractionInfoAzureTableStorageService>();
    });

host.Start();