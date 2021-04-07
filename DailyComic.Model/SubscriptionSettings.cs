namespace DailyComic.Model
{
    public class SubscriptionSettings
    {
        public SubscriptionName SubscriptionName { get; set; }

        public string WebhookUrl { get; set; }

        public IntegrationPlatform IntegrationPlatform { get; set; }
        public string SubscriptionId { get; set; }
    }
}