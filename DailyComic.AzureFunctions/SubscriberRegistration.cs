using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Web.Http;
using DailyComic.Contracts;
using DailyComic.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                SubscriptionSettings settings = await this.GetSettings(req);
                IComicRetriever retriever = ComicRetrieverFactory.Get(settings.SubscriptionName);
                ComicStrip comic = await retriever.GetComic();
                ComicSendingController sendingController = new ComicSendingController(comic);
                ComicDeliveryResult result = await sendingController.Push(settings);
                if (result.IsSuccess)
                {
                    await subscriberRegister.AddSubscriber(settings);
                    return new OkObjectResult("Subscribed");
                }
                else
                {
                    return new BadRequestErrorMessageResult("Error while sending test comic: " + result.Message);
                }

            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }

        private async Task<SubscriptionSettings> GetSettings(HttpRequest req)
        {
            using StreamReader streamReader = new StreamReader(req.Body);
            string body = await streamReader.ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(body);
            SubscriptionName subscriptionName = ParseEnum<SubscriptionName>(data.SubscriptionName.ToString());
            string url = data.WebhookUrl?.ToString();
            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                throw new ArgumentException($"Webhook URL seems invalid: {url}");
            }
            IntegrationPlatform platform = this.GetPlatform(url);

            return new SubscriptionSettings()
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                SubscriptionName = subscriptionName,
                IntegrationPlatform = platform,
                WebhookUrl = url
            };
        }

        private IntegrationPlatform GetPlatform(string url)
        {
            if (url.ToLowerInvariant().Contains("office.com"))
            {
                return IntegrationPlatform.Teams;
            }
            else if (url.ToLowerInvariant().Contains("slack.com"))
            {
                return IntegrationPlatform.Slack;
            }
            else
            {
                throw new ArgumentException(
                    "Provided webhook URL does not correspond to any of the supported platform");
            }
        }

        private static T ParseEnum<T>(string value) where T: struct 
        {
            try
            {
                return Enum.Parse<T>(value);
            }
            catch (Exception)
            {
                throw new FormatException($"Cannot parse {value} as {typeof(T).Name}. " +
                                          $"Available values: {string.Join(",",Enum.GetNames(typeof(T)))}");
            }
        }
    }
}