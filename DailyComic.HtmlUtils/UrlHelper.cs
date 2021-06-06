using System;
using System.Text;

namespace DailyComic.HtmlUtils
{
    public static class UrlHelper
    {
        public static string CombineUrls(params string[] parts)
        {
            var sb = new StringBuilder();
            foreach (string part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    sb.Append(part.Trim('/') + "/");
                }
            }
            return sb.ToString().TrimEnd('/');
        }
    }
}
