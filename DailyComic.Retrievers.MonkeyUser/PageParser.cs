using System;
using System.Collections.Generic;
using System.Linq;
using DailyComic.HtmlUtils;
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

        public string ParseInitialPageAndGetRandomUrl(string pageHtml)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(pageHtml);

            return GetComicUrlFromTocPage(document);
        }

        public ComicStrip Parse(string pageHtml, string finalUrl)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(pageHtml);

            ComicStrip comic = this.GetComicStripFromContainer(document, finalUrl);

            this.SetNextAndPreviousUrls(document, comic);
            
            this.SetTags(document, comic);

            this.SetBuyButton(comic);

            return comic;
        }

        private void SetBuyButton(ComicStrip comic)
        {
            comic.ExtraButtons.Add(new ExtraButton()
            {
                Location = ExtraButtonLocation.HeaderInline,
                Text = "BUY PLUSHIES",
                Url = "https://store.monkeyuser.com/"
            });

        }

        private void SetNextAndPreviousUrls(HtmlDocument document, ComicStrip comic)
        {
            string nextUrl = document.FirstWithClass("next")?.FirstHref();
            comic.NextUrl =  UrlHelper.CombineUrls(baseUrl, nextUrl);
            string prevUrl = document.FirstWithClass("prev")?.FirstHref();
            comic.PreviousUrl =  UrlHelper.CombineUrls(baseUrl, prevUrl);
        }

        private void SetTags(HtmlDocument document, ComicStrip comic)
        {
            IEnumerable<HtmlNode> tags = document.FirstWithClass("tags")?.DescendantsWithClass("tag");

            if (tags != null)
            {
                foreach (HtmlNode htmlNode in tags)
                {
                    var tag = new Tag()
                    {
                        Text = htmlNode.InnerText.Trim(),
                        Url = htmlNode.GetHref()
                    };
                    comic.Tags.Add(tag);
                }
            }
        }

        private static string GetComicUrlFromTocPage(HtmlDocument document)
        {
            var script = document.DocumentNode.Descendants("script").FirstOrDefault(x=>x.InnerHtml.Contains("posts.push("));
            var matches = System.Text.RegularExpressions.Regex.Matches(script?.InnerHtml??"", "(posts\\.push\\(\")([^\"]+)");
            
            if (matches.Count > 0)
            {
                var rnd = new Random();
                var match = matches.ElementAt(rnd.Next(0, matches.Count - 1));

                return match.Groups[2].Value;
            }
            else
            {
                throw new InvalidOperationException("Comic random button not found");
            }
        }

        private ComicStrip GetComicStripFromContainer(HtmlDocument document, string finalUrl)
        {
            HtmlNode container = document.FirstWithClass("post");
            var img = container?.FirstWithClass("content")?.First("img");

            if (img != null)
            {
                ComicStrip comic = new ComicStrip(ComicName.MonkeyUser)
                {
                    Title = img.Attributes["title"]?.Value,
                    PageUrl= UrlHelper.CombineUrls(this.baseUrl, finalUrl),
                    ImageUrl = img.Attributes["src"].Value,
                    Author = "MonkeyUser.com",
                    Date = container.First("time")?.InnerText,
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