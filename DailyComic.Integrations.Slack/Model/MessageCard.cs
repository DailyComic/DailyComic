using System.Collections.Generic;
using Newtonsoft.Json;

namespace DailyComic.Integrations.Slack.Model
{
    public partial class MessageCard
    {
        [JsonProperty("blocks")]
        public List<Block> Blocks { get; set; }
    }
}