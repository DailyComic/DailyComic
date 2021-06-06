using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Integrations.Slack.Model;
using DailyComic.Model;
using Newtonsoft.Json;

namespace DailyComic.Integrations.Slack
{
    public class SlackIntegration : IComicSender
    {
        public SlackIntegration(ComicStrip comic)
        {
            this.comic = comic;
            this.content = new Lazy<HttpContent>(this.GetContent);
        }

        private readonly ComicStrip comic;
        private readonly HttpClient client = new HttpClient();
        private readonly Lazy<HttpContent> content;
        private readonly IMessageCardCreator cardCreator = new SlackCardCreator();

        public async Task<ComicDeliveryResult> SendComicTo(SubscriptionSettings settings)
        {
            HttpResponseMessage response = await client.PostAsync(settings.WebhookUrl, content.Value);
            if (response.IsSuccessStatusCode)
            {
                return new ComicDeliveryResult() {IsSuccess = true};
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                return new ComicDeliveryResult() {IsSuccess = false, Message = errorResponse};
            }


        }

        private HttpContent GetContent()
        {
            MessageCard card = this.cardCreator.GetMessageCard(comic);
            string json = JsonConvert.SerializeObject(card);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }


    }
}
