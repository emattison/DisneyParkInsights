using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Refit;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DisneyWorldWaitTracker.Startup))]
namespace DisneyWorldWaitTracker
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddRefitClient<IThemeParksWiki>()
                            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.themeparks.wiki"));
        }
    }
}
