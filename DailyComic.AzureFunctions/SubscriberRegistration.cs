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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post","get", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                SubscriptionSettings settings = await this.GetSettings(req);
                await subscriberRegister.AddSubscriber(settings);
                return new OkObjectResult("Subscribed");
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult("Failed to register subscription: " + ex.Message);
            }

        }

        private async Task<SubscriptionSettings> GetSettings(HttpRequest req)
        {
            using StreamReader streamReader = new StreamReader(req.Body);
            string body = await streamReader.ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(body);
            SubscriptionName subscriptionName = ParseEnum<SubscriptionName>(data.SubscriptionName.ToString());
            IntegrationPlatform platform = ParseEnum<IntegrationPlatform>(data.IntegrationPlatform.ToString());
            string url = data.WebhookUrl?.ToString();
            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                throw new ArgumentException($"Webhook URL seems invalid: {url}");
            }

            return new SubscriptionSettings()
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                SubscriptionName = subscriptionName,
                IntegrationPlatform = platform,
                WebhookUrl = url
            };
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