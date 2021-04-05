using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Model;
using HtmlAgilityPack;

namespace DailyComic.Dilbert
{
    public class DilbertRetriever : IRandomComicRetriever, IComicOfTheDayRetriever
    {
        private HttpClient client;
        private readonly string baseUrl = "https://dilbert.com/";
        public DilbertRetriever()
        {
            this.client = new HttpClient() { BaseAddress = new Uri(baseUrl+"strip/") };
        }

        public async Task<ComicStrip> GetRandomComic()
        {
            HttpResponseMessage response = await this.client.GetAsync(this.GetDateString(true));
            response.EnsureSuccessStatusCode();


            string page = await response.Content.ReadAsStringAsync();

            return Parse(page);
        }

        private ComicStrip Parse(string pageHtml)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(pageHtml);

            ComicStrip comic = GetComicStripFromContainer(document);

            SetNextAndPreviousUrls(document, comic);

            return comic;
        }

        private void SetNextAndPreviousUrls(HtmlDocument document, ComicStrip comic)
        {
            HtmlNode nextUrl = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("js-load-comic-newer"));
            comic.NextUrl = this.baseUrl + nextUrl?.Attributes["href"]?.Value?.TrimStart('/');
            HtmlNode prevUrl = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("js-load-comic-older"));
            comic.PreviousUrl = this.baseUrl + prevUrl?.Attributes["href"]?.Value?.TrimStart('/');
        }

        private static ComicStrip GetComicStripFromContainer(HtmlDocument document)
        {
            HtmlNode container = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("comic-item-container"));

            if (container != null)
            {
                ComicStrip comic = new ComicStrip()
                {
                    ImageUrl = container.Attributes["data-image"]?.Value,
                    Title = container.Attributes["data-title"]?.Value,
                    PageUrl = container.Attributes["data-url"]?.Value,
                    Author = container.Attributes["data-creator"]?.Value,
                    Tags = container.Attributes["data-tags"]?.Value?.Split(","),
                    Date = container.Attributes["data-date"]?.Value
                };

                if (string.IsNullOrEmpty(comic.ImageUrl))
                {
                    throw new InvalidOperationException("Comic container does not contain image URL.");
                }

                return comic;
            }
            else
            {
                throw new InvalidOperationException("Comic container not found");
            }
        }

        public Task<ComicStrip> GetComicOfTheDay()
        {
            throw new NotImplementedException();
        }

        private string GetDateString(bool random)
        {
            if (random)
            {
                return FormatDate(this.GetRandomDate());
            }
            else
            {
                return FormatDate(DateTime.UtcNow);
            }
            string FormatDate(DateTime date)
            {
                return $"{date.Year}-{date.Month}-{date.Day}";
            }
        }

        private DateTime GetRandomDate()
        {
            Random dayRandomizer = new Random();
            DateTime randomComicMinimumDate = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - randomComicMinimumDate).Days;
            return randomComicMinimumDate.AddDays(dayRandomizer.Next(range));
        }
    }
}
