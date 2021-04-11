using Newtonsoft.Json;

namespace DailyComic.Integrations.Slack.Model
{
    public partial class Title
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("emoji")]
        public bool Emoji { get; set; }
    }
}