using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DailyComic.Integrations.Teams.Model;
using DailyComic.Model;
using Newtonsoft.Json;

namespace DailyComic.Integrations.Teams
{
    public class TeamsIntegration 
    {
        public TeamsIntegration(ComicStrip comic)
        {
            this.comic = comic;
            this.content = new Lazy<HttpContent>(this.GetContent);
            this.cardCreator = CardCreatorFactory.Get(comic);
        }

        private readonly ComicStrip comic;
        private readonly HttpClient client = new HttpClient();
        private readonly Lazy<HttpContent> content;
        private readonly IMessageCardCreator cardCreator;

        public async Task SendComicTo(SubscriptionSettings settings)
        {
            await client.PostAsync(settings.Url, content.Value);
        }

        private HttpContent GetContent()
        {
            MessageCard card = this.cardCreator.GetMessageCard(comic);
            string json = JsonConvert.SerializeObject(card);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }


    }
}
