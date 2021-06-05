using System;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Model;
using Polly;
using Polly.Retry;

namespace DailyComic.Retrievers.CommitStrip
{
    public abstract class RetrieverBase : IComicRetriever
    {
        protected RetrieverBase()
        {
            this.RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                });
            this.client = new PageLoaderWithRetries();
        }

        private readonly PageLoaderWithRetries client;
        protected readonly AsyncRetryPolicy RetryPolicy;

        public abstract Task<ComicStrip> GetComic();

        protected async Task<ComicStrip> GetComic(bool isRandomDate)
        {
            PageParser parser = new PageParser();
            if (isRandomDate)
            {
                return await GetRandomComic(parser);
            }
            else
            {
                ComicStrip comic;
                string dateString = GetDateString();
                string tocPage = await this.client.GetPageContentWithRetries($"https://" + $"www.commitstrip.com/en/{dateString}");
                string finalUrl = parser.ParseInitialPageAndGetUrl(tocPage);
                if (finalUrl != null)
                {
                    string finalPage = await this.client.GetPageContentWithRetries(finalUrl);
                    comic = parser.Parse(finalPage);
                }
                else
                {
                    comic= await GetRandomComic(parser);
                    comic.Title = $"{comic.Title} (No comics for {dateString})";
                }

                return comic;
            }
            
        }

        private async Task<ComicStrip> GetRandomComic(PageParser parser)
        {
            string finalPage = await this.client.GetPageContentWithRetries("https://www.commitstrip.com/en/?random=1");
            return parser.Parse(finalPage);
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
