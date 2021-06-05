using System;
using DailyComic.Contracts;
using DailyComic.Model;

namespace DailyComic.AzureFunctions
{
    public static class ComicRetrieverFactory
    {
        public static IComicRetriever Get(SubscriptionName subscriptionName)
        {
            switch (subscriptionName)
            {
                case SubscriptionName.CommitStripOfTheDay:
                    return new Retrievers.CommitStrip.ComicOfTheDayRetriever();
                case SubscriptionName.CommitStripRandom:
                    return new Retrievers.CommitStrip.RandomRetriever();

                //dilbert - not officially supported as no confirmation from the authors
                case SubscriptionName.DilbertRandom:
                    return new Retrievers.Dilbert.RandomRetriever();
                case SubscriptionName.DilbertOfTheDay:
                    return new Retrievers.Dilbert.ComicOfTheDayRetriever();
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionName), subscriptionName, $"Retriever for {subscriptionName} not implemented.");
            }
        }
    }
}