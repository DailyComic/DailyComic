using System;
using DailyComic.Model;

namespace DailyComic.Subscriptions.AzureStorage
{
    internal class SubscriptionMapper
    {
        public static SubscriptionEntity FromSettings(SubscriptionSettings subscriptionSettings)
        {
            SubscriptionEntity entity = new SubscriptionEntity
            {
                PartitionKey = subscriptionSettings.SubscriptionName.ToString(),
                RowKey = subscriptionSettings.SubscriptionId,
                WebhookUrl = subscriptionSettings.WebhookUrl,
                IntegrationPlatform = subscriptionSettings.IntegrationPlatform.ToString()
            };
            return entity;
        }

        public static SubscriptionSettings FromEntity(SubscriptionEntity entity)
        {
            SubscriptionSettings settings = new SubscriptionSettings()
            {
                SubscriptionName = Enum.Parse<SubscriptionName>(entity.PartitionKey),
                SubscriptionId = entity.RowKey,
                WebhookUrl = entity.WebhookUrl,
                IntegrationPlatform  = Enum.Parse<IntegrationPlatform>(entity.IntegrationPlatform),
            };
            return settings;
        }
    }
}