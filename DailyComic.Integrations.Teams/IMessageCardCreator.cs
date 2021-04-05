using DailyComic.Integrations.Teams.Model;
using DailyComic.Model;

namespace DailyComic.Integrations.Teams
{
    public interface IMessageCardCreator
    {
        public MessageCard GetMessageCard(ComicStrip comic);

    }
}