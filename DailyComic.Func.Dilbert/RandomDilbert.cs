using System;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Dilbert;
using DailyComic.Model;
using DailyComic.Subscriptions.AzureStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DailyComic.Func.Dilbert
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
        }

        [FunctionName("RandomDilbert")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", "get", Route = null)] HttpRequest req, ILogger log)
        //public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ComicStrip comic = await this.retriever.GetRandomComic();

            var subscriptions = subscriberProvider.GetSubscribers(SubscriptionName.RandomDilbert);

            await this.pusher.Push(comic);

            return new OkResult();
        }
    }
}
