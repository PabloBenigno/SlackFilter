namespace SlackFilter.ServiceClients.Model
{
    public class GetBuildDefinitionResult
    {
        public int Count { get; set; }
        public BuildDefinition[] Value { get; set; }
    }
}