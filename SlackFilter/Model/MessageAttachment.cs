using Newtonsoft.Json;

namespace SlackFilter.Model
{
    public class MessageAttachment
    {
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }
        [JsonProperty(PropertyName = "fields")]
        public MessageField[] Fields { get; set; }
        [JsonProperty(PropertyName = "pretext")]
        public string Pretext { get; set; }
        [JsonProperty(PropertyName = "mrkdwn_in")]
        public string[] MrkdwnIn { get; set; }
        [JsonProperty(PropertyName = "fallback")]
        public string Fallback { get; set; }
    }
}