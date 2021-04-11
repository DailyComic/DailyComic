using Newtonsoft.Json;

namespace DailyComic.Integrations.Slack.Model
{
    public partial class Text
    {
        [JsonProperty("text")]
        public string TextText { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}