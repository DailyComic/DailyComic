using Microsoft.WindowsAzure.Storage.Table;

namespace DailyComic.Subscriptions.AzureStorage
{
    public class SubscriptionEntity : TableEntity
    {
        public string IntegrationPlatform { get; set; }

        public string WebhookUrl { get; set; }

        public string SubscriptionSettings { get; set; }
    }
}