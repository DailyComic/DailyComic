namespace DailyComic.Model
{
    public class SubscriptionSettings
    {
        public SubscriptionName SubscriptionName { get; set; }

        public string Url { get; set; }

        public IntegrationPlatform IntegrationPlatform { get; set; }
    }
}