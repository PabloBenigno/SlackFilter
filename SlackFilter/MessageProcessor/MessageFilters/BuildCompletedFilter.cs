using System;
using System.Linq;
using SlackFilter.Configuration;
using SlackFilter.Model;
using SlackFilter.ServiceClients;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal class BuildCompletedFilter : IAttachmentFilter
    {
        private readonly TeamConfiguration _configuration;
        private readonly VstsClient _vstsClient;

        public BuildCompletedFilter(TeamConfiguration configuration, VstsClient vstsClient)
        {
            _configuration = configuration;
            _vstsClient = vstsClient;
        }
        public bool PassFilter(MessageAttachment attachment)
        {
            var buildDefinitionField = attachment.Fields.FirstOrDefault(_ => _.Title == "Build Definition");
            if (buildDefinitionField == null) return false;

            var buildDefinition = _vstsClient.GetBuildDefinitionByName(buildDefinitionField.Value);
            return buildDefinition.Path.TrimStart('\\').Equals(_configuration.BuildPath, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}