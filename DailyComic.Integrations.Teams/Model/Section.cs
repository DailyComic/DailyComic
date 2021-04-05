using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DailyComic.Integrations.Teams.Model
{
    public partial class Section
    {
        [JsonProperty("activityTitle")]
        public string ActivityTitle { get; set; }

        [JsonProperty("activitySubtitle")]
        public string ActivitySubtitle { get; set; }

        [JsonProperty("markdown")]
        public bool Markdown { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}