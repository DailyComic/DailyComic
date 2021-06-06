using System;
using System.Collections.Generic;
using System.Linq;
using DailyComic.Integrations.Slack.Model;
using DailyComic.Model;

namespace DailyComic.Integrations.Slack
{
    internal class SlackCardCreator : IMessageCardCreator
    {
        public virtual MessageCard GetMessageCard(ComicStrip comic)
        {
            MessageCard card = new MessageCard();

            RenderTitle(comic, card);

            RenderHeader(comic, card);

            RenderImageSection(comic, card);

            RenderTagsSection(comic, card);

            return card;
        }

        protected virtual void RenderHeader(ComicStrip comic, MessageCard card)
        {
            string extraButtonsMarkdown = "";
            List<ExtraButton> inlineButtons = comic.ExtraButtons.Where(x => x.Location == ExtraButtonLocation.HeaderInline).ToList();
            if (inlineButtons.Any())
            {
                foreach (ExtraButton inlineButton in inlineButtons)
                {
                    extraButtonsMarkdown += $" *|* {BuildLink(inlineButton.Text, inlineButton.Url)}";
                }

            }

            extraButtonsMarkdown += RenderSenderInfo();

            card.Blocks.Add(new Block()
            {
                Type = Types.Context,
                Elements = new List<Text>()
                {
                    new Text()
                    {
                        Type = Types.Markdown,
                        TextText = $"{BuildLink($"See on {GetDomain(comic)} :arrow_upper_right:", comic.PageUrl)} *|* " + GetNavigationButtons(comic) + extraButtonsMarkdown
                    }
                }
            });
        }

        private string RenderSenderInfo()
        {
            return $" *|* *_Sent with 😀 by {BuildLink("DailyComic", DailyComicUrls.LandingPageUrl)}_* ";
        }

        private static void RenderTitle(ComicStrip comic, MessageCard card)
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
        }

        protected virtual string GetNavigationButtons(ComicStrip comic)
        {
            if (!string.IsNullOrEmpty(comic.PreviousUrl))
            {
                return  $"{BuildLink(":arrow_left:", comic.PreviousUrl)} {BuildLink(":arrow_right:", comic.NextUrl)}";
            }

            return "";
        }

        private string BuildLink(string text, string url)
        {
            return $"<{url}|{text}>";
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
                    Elements = new List<Text>()
                    {
                        new Text()
                        {
                            Type = Types.Markdown,
                            TextText = RenderTags(),
                        },
                    }
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
    }
}