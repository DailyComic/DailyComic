using System;
using DailyComic.Contracts;
using DailyComic.Model;
using DailyComic.Retrievers.Dilbert;

namespace DailyComic.AzureFunctions
{
    public static class ComicRetrieverFactory
    {
        public static IComicRetriever Get(SubscriptionName subscriptionName)
        {
            switch (subscriptionName)
            {
                case SubscriptionName.RandomDilbert:
                    return new RandomRetriever();
                case SubscriptionName.DilbertOfTheDay:
                    return new ComicOfTheDayRetriever();
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionName), subscriptionName, $"Retriever for {subscriptionName} not implemented.");
            }
        }
    }
}