using System;
using System.Collections.Generic;
using System.Linq;
using DailyComic.Integrations.Teams.Model;
using DailyComic.Model;

namespace DailyComic.Integrations.Teams
{
    internal class CardCreator : IMessageCardCreator
    {
        public virtual MessageCard GetMessageCard(ComicStrip comic)
        {
            MessageCard card = new MessageCard(comic.Title);

            RenderHeader(comic, card);

            RenderImageSection(comic, card);

            RenderTagsSection(comic, card);

            return card;
        }

        protected virtual void RenderHeader(ComicStrip comic, MessageCard card)
        {
            card.Sections.Add(new Section()
            {
                Markdown = true,
                ActivityTitle = $"{comic.Title}",
                ActivitySubtitle = $"{comic.Date} | " +
                                   $"[See on {GetDomain(comic)} ⬈]({comic.PageUrl}) | " +
                                   $"[🡄]({comic.PreviousUrl}) [🡆]({comic.NextUrl})"
            });
        }

        protected virtual string GetDomain(ComicStrip comic)
        {
            try
            {
                var uri = new Uri(comic.PageUrl);
                return $"{uri.Host}";
            }
            catch (Exception)
            {
                return "original page";
            }
        }

        protected virtual void RenderImageSection(ComicStrip comic, MessageCard card)
        {
            card.Sections.Add(new Section()
            {
                Markdown = true,
                Text = $"![{comic.Title}]({comic.ImageUrl})",
            });
        }

        protected virtual void RenderTagsSection(ComicStrip comic, MessageCard card)
        {
            if (comic.Tags.Any())
            {
                card.Sections.Add(new Section()
                {
                    Markdown = true,
                    Text = RenderTags(comic),
                });
            }
            string RenderTags(ComicStrip comic)
            {
                List<string> tags = new List<string>();
                foreach (string tag in comic.Tags)
                {
                    tags.Add($"[#{tag}](https://dilbert.com/search_results?terms={tag.Replace(" ", "+")})");
                }
                return string.Join(", ", tags);
            }
        }
    }
}