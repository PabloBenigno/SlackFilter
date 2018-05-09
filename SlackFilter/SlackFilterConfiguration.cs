namespace SlackFilter
{
    public class SlackFilterConfiguration
    {
        public string SlackUrl { get; set; }
        public string RequesterList { get; set; }
        public string BuildList { get; set; }
        public string SuccessSuffix { get; set; }
        public string PartialSuccessSuffix { get; set; }
        public string FailSuffix { get; set; }
    }
}