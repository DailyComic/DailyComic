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
    public class RandomDilbert
    {
        private readonly IComicRetriever retriever;
        private readonly ISubscriberProvider subscriberProvider;

        public RandomDilbert(ISubscriberProvider subscriberProvider)
        {
            this.retriever = ComicRetrieverFactory.Get(SubscriptionName.DilbertRandom);
            this.subscriberProvider = subscriberProvider;
        }

        [FunctionName("RandomDilbert")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", "get", Route = null)] HttpRequest req, ILogger log)
        //public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ComicStrip comic = await this.retriever.GetComic();

            IEnumerable<SubscriptionSettings> subscriptions = await subscriberProvider.GetSubscribers(SubscriptionName.DilbertRandom);

            ComicSendingController sendingController = new ComicSendingController(comic);
            await sendingController.Push(subscriptions);

            return new OkResult();
        }
    }
}
