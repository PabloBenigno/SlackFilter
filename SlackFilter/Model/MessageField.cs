using Newtonsoft.Json;

namespace SlackFilter.Model
{
    public class MessageField
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
        [JsonProperty(PropertyName = "short")]
        public bool Short { get; set; }
    }
}