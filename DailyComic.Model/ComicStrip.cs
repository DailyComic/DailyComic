using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DailyComic.Model
{
    public class ComicStrip
    {
        public string PageUrl { get; set; }
        public string NextUrl { get; set; }
        public string PreviousUrl { get; set; }

        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; } = new List<string>().AsReadOnly();
        public string Author { get; set; }
        public string Date { get; set; }
    }
}
