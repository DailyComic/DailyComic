using System;
using System.Linq;
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
            HtmlNode nextUrl = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("nav-next"))?.Descendants("a").FirstOrDefault();
            comic.NextUrl =  nextUrl?.Attributes["href"]?.Value?.TrimStart('/');
            HtmlNode prevUrl = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("nav-previous"))?.Descendants("a").FirstOrDefault();
            comic.PreviousUrl = prevUrl?.Attributes["href"]?.Value?.TrimStart('/');
        }

        private static string GetComicUrlFromTocPage(HtmlDocument document)
        {
            HtmlNode container = document.DocumentNode.Descendants().FirstOrDefault(n => n.HasClass("excerpt"));
            
            if (container != null)
            {
                var url = container.Descendants("a").FirstOrDefault();
                if (url == null)
                {
                    throw new InvalidOperationException("Comic TOC URL not found");
                }

                return url.Attributes["href"].Value;
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
            HtmlNode container = document.DocumentNode.Descendants("article").FirstOrDefault();

            if (container != null)
            {
                ComicStrip comic = new ComicStrip(ComicName.CommitStrip)
                {
                    Title = container.Descendants().FirstOrDefault(x=>x.HasClass("entry-title"))?.InnerText,
                    PageUrl= container.Descendants().FirstOrDefault(x=>x.HasClass("entry-meta"))?.Descendants("a").FirstOrDefault()?.Attributes["href"]?.Value,
                    ImageUrl = container.Descendants("img").FirstOrDefault()?.Attributes["src"].Value,
                    Author = "CommitStrip.com",
                    Date = container.Descendants().FirstOrDefault(x => x.HasClass("entry-date"))?.InnerText,
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