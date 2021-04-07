using System;
using DailyComic.Contracts;
using DailyComic.Subscriptions.AzureStorage;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DailyComic.AzureFunctions
{
    public static class SubscriptionsControllerStartup
    {
        public static void AddSubscriptionsController(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ISubscriberRegister>(provider =>
            {
                DailyComicSettings settings = provider.GetService<IOptions<DailyComicSettings>>()?.Value;
                if (settings == null) throw new ArgumentNullException(nameof(settings));
                return new AzureStorageSubscriptionController(settings.StorageConnectionString,
                    provider.GetService<ILogger<AzureStorageSubscriptionController>>());
            });
            builder.Services.AddSingleton<ISubscriberProvider>(provider =>
            {
                DailyComicSettings settings = provider.GetService<IOptions<DailyComicSettings>>()?.Value;
                if (settings == null) throw new ArgumentNullException(nameof(settings));
                return new AzureStorageSubscriptionController(settings.StorageConnectionString,
                    provider.GetService<ILogger<AzureStorageSubscriptionController>>());
            });
        }
    }
}