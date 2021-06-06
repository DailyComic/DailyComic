using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DailyComic.Model
{
    public class ComicStrip
    {
        public ComicStrip(ComicName comicName)
        {
            ComicName = comicName;
        }
        public ComicName ComicName { get; }
        public string PageUrl { get; set; }
        public string NextUrl { get; set; }
        public string PreviousUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public string Author { get; set; }
        public string Date { get; set; }
        public string ComicId { get; set; }
        public List<ExtraButton> ExtraButtons { get; set; } = new List<ExtraButton>();

    }
}
