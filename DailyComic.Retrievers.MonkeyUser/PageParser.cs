using System;
using System.Linq;
using DailyComic.Model;
using HtmlAgilityPack;

namespace DailyComic.Retrievers.MonkeyUser
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

            return comic;
        }

        private void SetNextAndPreviousUrls(HtmlDocument document, ComicStrip comic)
        {
            HtmlNode nextUrl = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("js-load-comic-newer"));
            comic.NextUrl = this.baseUrl + nextUrl?.Attributes["href"]?.Value?.TrimStart('/');
            HtmlNode prevUrl = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("js-load-comic-older"));
            comic.PreviousUrl = this.baseUrl + prevUrl?.Attributes["href"]?.Value?.TrimStart('/');
        }

        private static ComicStrip GetComicStripFromContainer(HtmlDocument document)
        {
            HtmlNode container = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("comic-item-container"));

            if (container != null)
            {
                ComicStrip comic = new ComicStrip(ComicName.Dilbert)
                {
                    ImageUrl = container.Attributes["data-image"]?.Value,
                    Title = container.Attributes["data-title"]?.Value,
                    PageUrl = container.Attributes["data-url"]?.Value,
                    ComicId = container.Attributes["data-id"]?.Value,
                    Author = container.Attributes["data-creator"]?.Value,
                    Tags = container.Attributes["data-tags"]?.Value?.Split(","),
                    Date = container.Attributes["data-date"]?.Value
                };

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
    }
}