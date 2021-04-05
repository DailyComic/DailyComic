using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DailyComic.Func.Dilbert
{
    public static class DilbertOfTheDay
    {
        [FunctionName("DilbertOfTheDay")]
        public static void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}