using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Integrations.Teams;
using DailyComic.Model;

namespace DailyComic.AzureFunctions
{
    internal class ComicPusher 
    {
        private readonly TeamsIntegration teams;
        public ComicPusher(ComicStrip comic)
        {
            teams = new TeamsIntegration(comic);
        }

        public async Task Push(IEnumerable<SubscriptionSettings> subscriptionSettings)
        {
            List<Task> tasks = new List<Task>();
            foreach (SubscriptionSettings settings in subscriptionSettings)
            {
                tasks.Add(StartDeliveryTask(settings));  
            }

            await Task.WhenAll(tasks);
        }

        private Task StartDeliveryTask(SubscriptionSettings settings)
        {
            switch (settings.IntegrationPlatform)
            {
                case IntegrationPlatform.Teams:
                    return teams.SendComicTo(settings);
                case IntegrationPlatform.Slack:
                    throw new NotImplementedException("Slack is not there yet");
                default:
                    throw new ArgumentOutOfRangeException($"{settings.IntegrationPlatform} is not supported.");
            }
        }
    }
}