using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Refit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using DisneyParkInsights;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(DisneyWorldWaitTracker.Startup))]
namespace DisneyWorldWaitTracker
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddRefitClient<IThemeParksWiki>()
                            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.themeparks.wiki"));

            builder.Services.AddOptions<AttractionInfoAzureTableStorageConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(AttractionInfoAzureTableStorageConfig)).Bind(settings);
                });

            builder.Services.AddTransient<IAttractionInfoStorageService, AttractionInfoAzureTableStorageService>();
        }
    }
}
