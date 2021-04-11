using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Integrations.Slack;
using DailyComic.Integrations.Teams;
using DailyComic.Model;

namespace DailyComic.AzureFunctions
{
    internal class ComicSendingController 
    {
        private readonly TeamsIntegration teams;
        private readonly SlackIntegration slack;
        public ComicSendingController(ComicStrip comic)
        {
            teams = new TeamsIntegration(comic);
            slack = new SlackIntegration(comic);

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

        public Task<ComicDeliveryResult> Push(SubscriptionSettings settings)
        {
            return StartDeliveryTask(settings);
        }

        private Task<ComicDeliveryResult>StartDeliveryTask(SubscriptionSettings settings)
        {
            switch (settings.IntegrationPlatform)
            {
                case IntegrationPlatform.Teams:
                    return teams.SendComicTo(settings);
                case IntegrationPlatform.Slack:
                    return slack.SendComicTo(settings);
                default:
                    throw new ArgumentOutOfRangeException($"{settings.IntegrationPlatform} is not supported.");
            }
        }
    }
}