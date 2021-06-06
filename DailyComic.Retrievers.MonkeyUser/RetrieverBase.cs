using System;
using System.Net.Http;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Model;
using Polly;
using Polly.Retry;

namespace DailyComic.Retrievers.MonkeyUser
{
    public abstract class RetrieverBase : IComicRetriever
    {
        protected RetrieverBase()
        {
            this.client = new HttpClient() { BaseAddress = new Uri(BaseUrl) };
            this.RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                });
        }

        private readonly HttpClient client;
        protected readonly string BaseUrl = "https://www.monkeyuser.com/";
        protected readonly AsyncRetryPolicy RetryPolicy;

        public abstract Task<ComicStrip> GetComic();

        protected async Task<ComicStrip> GetComic(bool isRandomDate)
        {
            PageParser parser = new PageParser(this.BaseUrl);
            if (isRandomDate)
            {
                return await GetRandomComic(parser);
            }
            else
            {
                //ComicStrip comic;
                //string dateString = GetDateString();
                //string tocPage = await this.client.GetPageContentWithRetries($"https://" + $"www.commitstrip.com/en/{dateString}");
                //string finalUrl = parser.ParseInitialPageAndGetRandomUrl(tocPage);
                //if (finalUrl != null)
                //{
                //    string finalPage = await this.client.GetPageContentWithRetries(finalUrl);
                //    comic = parser.Parse(finalPage);
                //}
                //else
                //{
                //    comic = await GetRandomComic(parser);
                //    comic.Title = $"{comic.Title} (No comics for {dateString})";
                //}

                return null;
            }

        }

        private async Task<ComicStrip> GetRandomComic(PageParser parser)
        {
            HttpResponseMessage homePageResponse = await this.client.GetAsync("");
            string homePage = await homePageResponse.Content.ReadAsStringAsync();

            string finalUrl = parser.ParseInitialPageAndGetRandomUrl(homePage);

            HttpResponseMessage finalResponse = await this.client.GetAsync(finalUrl);

            string comicPage = await finalResponse.Content.ReadAsStringAsync();

            return parser.Parse(comicPage, finalUrl);
        }

        private string GetDateString()
        {
            return FormatDate(DateTime.UtcNow);
            string FormatDate(DateTime date)
            {
                return $"{date.Year}/{date.Month}/{date.Day}";
            }
        }

    }
}
