using System;
using System.Linq;
using DailyComic.HtmlUtils;
using DailyComic.Model;
using HtmlAgilityPack;

namespace DailyComic.Retrievers.Dilbert
{
    internal class PageParser
    {
        private readonly string baseUrl;

        public PageParser(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public ComicStrip Parse(string pageHtml)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(pageHtml);

            ComicStrip comic = GetComicStripFromContainer(document);

            this.SetNextAndPreviousUrls(document, comic);

            this.AddBuyButton(comic);

            return comic;
        }

        private void AddBuyButton(ComicStrip comic)
        {
            var buyButton = new ExtraButton()
            {
                Text = "BUY",
                Url = $"https://" + $"dilbert.com/buy?date={comic.ComicId}",
                Location = ExtraButtonLocation.HeaderInline
            };

            comic.ExtraButtons.Add(buyButton);
        }

        private void SetNextAndPreviousUrls(HtmlDocument document, ComicStrip comic)
        {
            HtmlNode nextUrl = document.FirstWithClass("js-load-comic-newer");
            comic.NextUrl = UrlHelper.CombineUrls(this.baseUrl, nextUrl.GetHref());
            
            HtmlNode prevUrl = document.FirstWithClass("js-load-comic-older");
            comic.PreviousUrl= UrlHelper.CombineUrls(this.baseUrl, prevUrl.GetHref());
        }

        private static ComicStrip GetComicStripFromContainer(HtmlDocument document)
        {
            HtmlNode container = document.FirstWithClass("comic-item-container");

            if (container != null)
            {
                ComicStrip comic = new ComicStrip(ComicName.Dilbert)
                {
                    ImageUrl = container.GetAttr("data-image"),
                    Title   = container.GetAttr("data-title"),
                    PageUrl = container.GetAttr("data-url"),
                    ComicId = container.GetAttr("data-id"),
                    Author  = container.GetAttr("data-creator"),
                    Date    = container.GetAttr("data-date")
                };

                SetTags(comic, container);

                if (string.IsNullOrEmpty(comic.ImageUrl))
                {
                    throw new InvalidOperationException("Comic container does not contain image URL.");
                }

                return comic;
            }
            else
            {
                throw new InvalidOperationException("Comic container not found");
            }
        }

        private static void SetTags(ComicStrip comic, HtmlNode container)
        {
            string[] tags = container.GetAttr("data-tags")?.Split(",");
            if (tags != null)
            {
                foreach (string tagText in tags)
                {
                    Tag tag = new Tag()
                    {
                        Text = tagText,
                        Url = $"https://" + $"dilbert.com/search_results?terms={tagText.Replace(" ", "+")}"
                    };
                    comic.Tags.Add(tag);
                }
            }
        }
    }
}