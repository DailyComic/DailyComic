#nullable enable
using System;
using DailyComic.Contracts;
using DailyComic.Subscriptions.AzureStorage;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DailyComic.AzureFunctions
{
    public static class SubscriptionsControllerStartup
    {
        public static void AddSubscriptionsController(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ISubscriberRegister>(provider =>
            {
                return new AzureStorageSubscriptionController(GetStorageConnectionString(provider),
                    provider.GetService<ILogger<AzureStorageSubscriptionController>>());
            });
            builder.Services.AddSingleton<ISubscriberProvider>(provider =>
            {
                return new AzureStorageSubscriptionController(GetStorageConnectionString(provider),
                    provider.GetService<ILogger<AzureStorageSubscriptionController>>());
            });
        }

        private static string GetStorageConnectionString(IServiceProvider provider)
        {
            IConfiguration? cfg = provider.GetService<IConfiguration>();
            if (cfg == null)
            {
                throw new InvalidOperationException("Azure function configuration is null!");
            }

            string connectionString = cfg["AzureWebJobsStorage"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("AzureWebJobsStorage configuration value is null. Connection string required.");
            }
            return connectionString;
        }
    }
}