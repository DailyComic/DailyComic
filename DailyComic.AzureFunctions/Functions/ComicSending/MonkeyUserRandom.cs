using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Model;
using DailyComic.Retrievers.Dilbert;
using DailyComic.Subscriptions.AzureStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DailyComic.AzureFunctions
{
    // ReSharper disable once UnusedMember.Global
    public class MonkeyUserRandom
    {
        private readonly IComicRetriever retriever;
        private readonly ISubscriberProvider subscriberProvider;
        private readonly SubscriptionName subscriptionName = SubscriptionName.MonkeyUserRandom;

        public MonkeyUserRandom(ISubscriberProvider subscriberProvider)
        {
            this.retriever = ComicRetrieverFactory.Get(subscriptionName);
            this.subscriberProvider = subscriberProvider;
        }

        [FunctionName("MonkeyUserRandom")]
        public async Task Run([TimerTrigger("0 0 7 * * 1-5")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{this.GetType().Name} - Starting function execution");

            ComicStrip comic = await this.retriever.GetComic();

            IEnumerable<SubscriptionSettings> subscriptions = await subscriberProvider.GetSubscribers(subscriptionName);

            ComicSendingController sendingController = new ComicSendingController(comic);
            await sendingController.Push(subscriptions);
        }
    }
}
