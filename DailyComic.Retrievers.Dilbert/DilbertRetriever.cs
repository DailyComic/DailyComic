using System;
using System.Net.Http;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Model;
using Polly;
using Polly.Retry;

namespace DailyComic.Retrievers.Dilbert
{
    public class DilbertRetriever : IRandomComicRetriever, IComicOfTheDayRetriever
    {
        public DilbertRetriever()
        {
            this.client = new HttpClient() { BaseAddress = new Uri(baseUrl + "strip/") };
            this.retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                });
        }

        private readonly HttpClient client;
        private readonly string baseUrl = "https://dilbert.com/";
        private readonly AsyncRetryPolicy retryPolicy;

        public Task<ComicStrip> GetRandomComic()
        {
            return retryPolicy.ExecuteAsync(async () => await GetComic(true));
        }

        public Task<ComicStrip> GetComicOfTheDay()
        {
            return retryPolicy.ExecuteAsync(async ()=>await GetComic(false));
        }

        private async Task<ComicStrip> GetComic(bool isRandomDate)
        {
            HttpResponseMessage response = await this.client.GetAsync(GetDateString(isRandomDate));
            response.EnsureSuccessStatusCode();
            string page = await response.Content.ReadAsStringAsync();

            return new PageParser(this.baseUrl).Parse(page);
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
