using DailyComic.AzureFunctions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DailyComic.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<DailyComicSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(DailyComicSettings)).Bind(settings);
                });

            builder.AddSubscriptionsController();
        }


    }
}