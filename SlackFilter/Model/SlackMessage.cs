using Newtonsoft.Json;

namespace SlackFilter.Model
{
    public class SlackMessage
    {
        [JsonProperty(PropertyName = "attachments")]
        public MessageAttachment[] Attachments { get; set; }
    }
}