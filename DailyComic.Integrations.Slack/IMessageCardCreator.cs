using DailyComic.Integrations.Slack.Model;
using DailyComic.Model;

namespace DailyComic.Integrations.Slack
{
    public interface IMessageCardCreator
    {
        public MessageCard GetMessageCard(ComicStrip comic);

    }
}