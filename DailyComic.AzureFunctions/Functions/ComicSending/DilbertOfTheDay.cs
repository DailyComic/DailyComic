using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DailyComic.AzureFunctions
{
    public static class DilbertOfTheDay
    {
        [FunctionName("DilbertOfTheDay")]
        public static void Run([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}