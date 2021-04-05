using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DailyComic.Integrations.Teams.Model
{
    public partial class MessageCard
    {
        public MessageCard(string summary)
        {
            Summary = summary;
        }

        [JsonProperty("@type")]
        public string Type { get; set; } = "MessageCard";

        [JsonProperty("@context")]
        public Uri Context { get; set; } = new Uri("http://schema.org/extensions");

        [JsonProperty("themeColor")]
        public string ThemeColor { get; set; } = "0076D7";

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("sections")]
        public List<Section> Sections { get; set; } = new List<Section>();
    }
}