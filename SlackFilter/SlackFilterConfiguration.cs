namespace SlackFilter
{
    public class SlackFilterConfiguration
    {
        public TeamConfiguration[] TeamConfigurations { get; set; }
    }

    public class TeamConfiguration
    {
        public string Name { get; set; }
        public string SlackUrl { get; set; }
        public string[] RequesterList { get; set; }
        public string[] BuildList { get; set; }
        public MessageTransformation MessageTransformation { get; set; }
    }

    public class MessageTransformation
    {
        public string SuccessSuffix { get; set; }
        public string PartialSuccessSuffix { get; set; }
        public string FailSuffix { get; set; }
    }
}