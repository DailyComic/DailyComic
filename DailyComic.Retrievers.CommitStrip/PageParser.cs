using System;
using System.Linq;
using DailyComic.HtmlUtils;
using DailyComic.Model;
using HtmlAgilityPack;

namespace DailyComic.Retrievers.CommitStrip
{
    internal class PageParser
    {

        public PageParser()
        {
        }

        public string ParseInitialPageAndGetUrl(string pageHtml)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(pageHtml);

            return GetComicUrlFromTocPage(document);
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
            comic.NextUrl = document.FirstWithClass("nav-next")?.FirstHref();
            comic.PreviousUrl = document.FirstWithClass("nav-previous")?.FirstHref();
        }

        private static string GetComicUrlFromTocPage(HtmlDocument document)
        {
            HtmlNode container = document.FirstWithClass("excerpt");
            
            if (container != null)
            {
                var url = container.FirstHref();
                if (url == null)
                {
                    throw new InvalidOperationException("Comic TOC URL not found");
                }

                return url;
            }
            else
            {
                if (document.DocumentNode.Descendants().Any(x => x.HasClass("error404")))
                {
                    return null;
                }
                throw new InvalidOperationException("Comic TOC not found");
            }
        }

        private static ComicStrip GetComicStripFromContainer(HtmlDocument document)
        {
            HtmlNode container = document.First("article");

            if (container != null)
            {
                ComicStrip comic = new ComicStrip(ComicName.CommitStrip)
                {
                    Title = container.FirstWithClass("entry-title")?.InnerText?.Trim(),
                    PageUrl= container.FirstWithClass("entry-meta").FirstHref(),
                    ImageUrl = container.First("img").GetAttr("src"),
                    Author = "CommitStrip.com",
                    Date = container.FirstWithClass("entry-date")?.InnerText,
                };
                comic.ComicId = comic.PageUrl;

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