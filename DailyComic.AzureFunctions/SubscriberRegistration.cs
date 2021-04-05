using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DailyComic.AzureFunctions
{
    public class SubscriberRegistration
    {
        private readonly ISubscriberRegister subscriberRegister;

        public SubscriberRegistration(ISubscriberRegister subscriberRegister)
        {
            this.subscriberRegister = subscriberRegister;
        }

        [FunctionName("SubscriberRegistration")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post","get", Route = null)] HttpRequest req, ILogger log)
        {
            SubscriptionSettings settings = this.GetSettings(req);

            await subscriberRegister.AddSubscriber(settings);

            return new OkObjectResult("Subscribed");
        }

        private SubscriptionSettings GetSettings(HttpRequest req)
        {
            return new SubscriptionSettings()
            {
                SubscriptionName = SubscriptionName.RandomDilbert,
                IntegrationPlatform = IntegrationPlatform.Teams,
                Url = "https://sdl365.webhook.office.com/webhookb2/99d55d1c-8d2f-4347-913d-003b0b320bc5@df02c2f8-e418-484f-8bd6-c7f2e154f292/IncomingWebhook/82de6b302aba414f92fff698fe3b6dd4/85f91934-34b4-4405-a991-6afdf5cb965c"
            };
        }

    }
}