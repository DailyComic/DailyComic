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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ComicStrip comic = await this.retriever.GetComic();

            IEnumerable<SubscriptionSettings> subscriptions = await subscriberProvider.GetSubscribers(subscriptionName);

            ComicSendingController sendingController = new ComicSendingController(comic);
            await sendingController.Push(subscriptions);

            return new OkResult();
        }
    }
}
