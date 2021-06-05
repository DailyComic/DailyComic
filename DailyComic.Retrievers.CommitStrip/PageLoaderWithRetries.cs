using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace DailyComic.Retrievers.CommitStrip
{
    class PageLoaderWithRetries
    {
        public PageLoaderWithRetries()
        {
            this.retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromMilliseconds(300),
                    TimeSpan.FromMilliseconds(500),
                    TimeSpan.FromMilliseconds(800),
                    TimeSpan.FromMilliseconds(1500)
                });
            HttpClientHandler handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            this.client = new HttpClient(handler) { };
        }

        private readonly AsyncRetryPolicy retryPolicy;
        private readonly HttpClient client;

        public async Task<string> GetPageContentWithRetries(string url)
        {
            string result = await this.retryPolicy.ExecuteAsync(async () => await GetPageContent(url));
            return result;
        }

        private async Task<string> GetPageContent(string url)
        {
            HttpResponseMessage response = await this.client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.Found)
            {
                url = response.Headers.Location.ToString();
                response = await this.client.GetAsync(url);
            }

            return await response.Content.ReadAsStringAsync();
        }

    }
}