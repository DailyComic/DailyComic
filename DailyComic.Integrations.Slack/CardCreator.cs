using System;
using System.Collections.Generic;
using System.Linq;
using DailyComic.Integrations.Slack.Model;
using DailyComic.Model;

namespace DailyComic.Integrations.Slack
{
    internal class CardCreator : IMessageCardCreator
    {
        public virtual MessageCard GetMessageCard(ComicStrip comic)
        {
            MessageCard card = new MessageCard();

            RenderHeader(comic, card);

            RenderImageSection(comic, card);

            RenderTagsSection(comic, card);

            return card;
        }

        protected virtual void RenderHeader(ComicStrip comic, MessageCard card)
        {
            card.Blocks.Add(new Block()
            {
                Type = Types.Header,
                Text = new Text()
                {
                    TextText = $"{comic.Title}",
                    Type = Types.PlainText
                }
            });
            card.Blocks.Add(new Block()
            {
                Type = Types.Context,
                Elements = new List<Text>()
                {
                    new Text()
                    {
                        Type = Types.Markdown,
                        TextText= $"<{comic.PageUrl}|See on {GetDomain(comic)} :arrow_upper_right:> | " +
                        GetNavigationButtons(comic)
                    }
                }
            });
        }

        protected virtual string GetNavigationButtons(ComicStrip comic)
        {
            if (!string.IsNullOrEmpty(comic.PreviousUrl))
            {
                return $"<{comic.PreviousUrl}|:arrow_left:> <{comic.NextUrl}|:arrow_right:>";
            }

            return "";
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
            card.Blocks.Add(new Block()
            {
                Type = Types.Image,
                Title = new Title()
                {
                    Type = Types.PlainText,
                    Text = comic.Date
                },
                ImageUrl = new Uri(comic.ImageUrl),
                AltText = comic.Title
            });
        }


        protected virtual void RenderTagsSection(ComicStrip comic, MessageCard card)
        {
            if (comic.Tags.Any())
            {
                card.Blocks.Add(new Block()
                {
                    Type = Types.Divider
                });
                card.Blocks.Add(new Block()
                {
                    Type = Types.Context,
                    Text = new Text()
                    {
                        Type = Types.Markdown,
                        TextText = RenderTags(),
                    }
                });
            }
            string RenderTags()
            {
                List<string> tags = new List<string>();
                foreach (var tag in comic.Tags)
                {
                    tags.Add($"<{tag.Url}|#{tag.Text}>");
                }
                return string.Join(", ", tags);
            }
        }
    }
}