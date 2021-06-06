using System;
using System.Collections.Generic;
using System.Linq;
using DailyComic.Integrations.Teams.Model;
using DailyComic.Model;

namespace DailyComic.Integrations.Teams
{
    internal class TeamsCardCreator : IMessageCardCreator
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
            Section header = new Section()
            {
                Markdown = true,
                ActivityTitle = $"{comic.Title}",
                ActivitySubtitle = $"{comic.Date} | " +
                                   $"{BuildLink($"See on {GetDomain(comic)} ⬈", comic.PageUrl)} | " +
                                   $"{BuildLink($"🡄", comic.PreviousUrl)} | {BuildLink($"🡆", comic.NextUrl)}" 
            };
            List<ExtraButton> inlineButtons = comic.ExtraButtons.Where(x => x.Location == ExtraButtonLocation.HeaderInline).ToList();
            if (inlineButtons.Any())
            {
                foreach (ExtraButton inlineButton in inlineButtons)
                {
                    header.ActivitySubtitle += $" | {BuildLink(inlineButton.Text, inlineButton.Url)}";
                }
            }

            header.ActivitySubtitle += this.AddSenderInfo();
            card.Sections.Add(header);
        }

        private string AddSenderInfo()
        {
           return $" **| _Sent with_** 😀 **_by {BuildLink("DailyComic", DailyComicUrls.LandingPageUrl)}_**";
        }


        protected virtual string GetDomain(ComicStrip comic)
        {
            try
            {
                Uri uri = new Uri(comic.PageUrl);
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
                Text = $"![{comic.Title} (if comic is not visible, perhaps it's too 'large/heavy' for Teams, sorry)]({comic.ImageUrl})",
            });
        }

        protected virtual void RenderTagsSection(ComicStrip comic, MessageCard card)
        {
            if (comic.Tags.Any())
            {
                card.Sections.Add(new Section()
                {
                    Markdown = true,
                    Text = RenderTags() ,
                });
            }
            string RenderTags()
            {
                List<string> tags = new List<string>();
                foreach (Tag tag in comic.Tags)
                {
                    tags.Add($"{BuildLink($"#{tag.Text}", tag.Url)}");
                }
                return string.Join(", ", tags);
            }
        }

        private string BuildLink(string text, string url)
        {
            return $"[{text}]({url})";
        }
    }
}