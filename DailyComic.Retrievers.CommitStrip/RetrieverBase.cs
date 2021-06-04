using System;
using System.Net.Http;
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
            this.client = new HttpClient() { BaseAddress = new Uri(BaseUrl + "en/") };
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
        protected readonly string BaseUrl = "https://www.commitstrip.com/";
        protected readonly AsyncRetryPolicy RetryPolicy;

        public abstract Task<ComicStrip> GetComic();

        protected async Task<ComicStrip> GetComic(bool isRandomDate)
        {
            HttpResponseMessage response = await this.client.GetAsync(GetDateString(isRandomDate));
            response.EnsureSuccessStatusCode();
            string page = await response.Content.ReadAsStringAsync();

            return new PageParser(this.BaseUrl).Parse(page);
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
