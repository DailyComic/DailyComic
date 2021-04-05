using System.Text;
using DailyComic.Integrations.Teams.Model;
using DailyComic.Model;

namespace DailyComic.Integrations.Teams
{

    internal class DilbertCardCreator : CardCreator
    {
        protected override void RenderHeader(ComicStrip comic, MessageCard card)
        {
            card.Sections.Add(new Section()
            {
                Markdown = true,
                ActivityTitle = $"{comic.Title}",
                ActivitySubtitle = $"{comic.Date} | " +
                                   $"[See on {GetDomain(comic)} ⬈]({comic.PageUrl}) | " +
                                   $"[🡄]({comic.PreviousUrl}) [🡆]({comic.NextUrl}) | " +
                                   $"[BUY](https://dilbert.com/buy?date={comic.ComicId})"
            });
        }
    }
}