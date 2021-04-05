using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Model;

namespace DailyComic.Subscriptions.AzureStorage
{
    public class AzureStorageSubscriptionController : ISubscriberProvider, ISubscriberRegister
    {
        public async Task<IEnumerable<SubscriptionSettings>> GetSubscribers(SubscriptionName subscriptionName)
        {
            await Task.Delay(0);
            return new List<SubscriptionSettings>()
            {
                new SubscriptionSettings()
                {
                    Url =
                        "https://sdl365.webhook.office.com/webhookb2/99d55d1c-8d2f-4347-913d-003b0b320bc5@df02c2f8-e418-484f-8bd6-c7f2e154f292/IncomingWebhook/82de6b302aba414f92fff698fe3b6dd4/85f91934-34b4-4405-a991-6afdf5cb965c",
                    PlatformName = "Teams",
                    SubscriptionName = SubscriptionName.RandomDilbert
                }
            };
        }

        public Task<IEnumerable<SubscriptionSettings>> AddSubscriber(SubscriptionSettings subscriptionName)
        {
            throw new NotImplementedException();
        }
    }
}
