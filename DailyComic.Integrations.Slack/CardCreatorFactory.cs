using DailyComic.Model;

namespace DailyComic.Integrations.Slack
{
    internal static class CardCreatorFactory
    {
        public static IMessageCardCreator Get(ComicStrip comic)
        {
            if (comic.ComicName == ComicName.Dilbert)
            {
                return new DilbertCardCreator();
            }

            return new CardCreator();
        }
    }
}