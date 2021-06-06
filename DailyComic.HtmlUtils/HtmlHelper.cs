using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace DailyComic.HtmlUtils
{
    public static class HtmlHelper
    {
        public static HtmlNode First(this HtmlNode node, string tagName)
        {
            return node.Descendants(tagName).FirstOrDefault();
        }

        public static HtmlNode FirstWithClass(this HtmlNode node, string className)
        {
            return node.Descendants().FirstOrDefault(n => n.HasClass(className));
        }

        public static HtmlNode FirstWithTagAndClass(this HtmlNode node, string tagName, string className)
        {
            return node.Descendants(tagName).FirstOrDefault(n => n.HasClass(className));
        }

        public static HtmlNode First(this HtmlDocument document, string tagName)
        {
            return document.DocumentNode.Descendants(tagName).FirstOrDefault();
        }

        public static HtmlNode FirstWithClass(this HtmlDocument document, string className)
        {
            return document.DocumentNode.FirstWithClass(className);
        }
        
        public static IEnumerable<HtmlNode> DescendantsWithClass(this HtmlNode node, string className)
        {
            return node.Descendants().Where(x => x.HasClass(className));
        }

        public static HtmlNode FirstWithTagAndClass(this HtmlDocument document, string tagName, string className)
        {
            return document.DocumentNode.FirstWithTagAndClass(tagName, className);
        }

        public static string GetHref(this HtmlNode urlNode)
        {
            return urlNode?.Attributes["href"]?.Value;
        }

        public static string FirstHref(this HtmlNode urlNode)
        {
            return urlNode.Descendants("a").FirstOrDefault()?.GetHref();
        }

        public static string GetAttr(this HtmlNode node, string attrName)
        {
            return node?.Attributes[attrName]?.Value;
        }
    }
}
