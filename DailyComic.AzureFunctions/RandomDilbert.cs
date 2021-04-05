using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Integrations.Teams;
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
    public class RandomDilbert
    {
        private readonly IRandomComicRetriever retriever;
        private readonly IComicPusher pusher;
        private readonly ISubscriberProvider subscriberProvider;

        public RandomDilbert()
        {
            this.retriever = new DilbertRetriever();
            this.subscriberProvider = new AzureStorageSubscriptionController();
            this.pusher = new ComicPusher();
        }

        [FunctionName("RandomDilbert")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", "get", Route = null)] HttpRequest req, ILogger log)
        //public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ComicStrip comic = await this.retriever.GetRandomComic();

            IEnumerable<SubscriptionSettings> subscriptions = await subscriberProvider.GetSubscribers(SubscriptionName.RandomDilbert);

            await this.pusher.Push(comic, subscriptions);

            return new OkResult();
        }
    }

    public class ComicPusher : IComicPusher
    {
        public async Task Push(ComicStrip comic, IEnumerable<SubscriptionSettings> subscriptionSettingsEnumerable)
        {
            TeamsIntegration integration = new TeamsIntegration(comic);

            var tasks = new List<Task>();
            foreach (SubscriptionSettings subscriptionSettings in subscriptionSettingsEnumerable)
            {
                tasks.Add(integration.SendComicTo(subscriptionSettings));  
            }

            await Task.WhenAll(tasks);
        }
    }
}
