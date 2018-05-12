using System;
using System.Linq;
using SlackFilter.Configuration;
using SlackFilter.Model;
using SlackFilter.ServiceClients;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal class ReleaseCompletedFilter : IAttachmentFilter
    {
        private readonly TeamConfiguration _configuration;
        private readonly VstsClient _vstsClient;

        public ReleaseCompletedFilter(TeamConfiguration configuration, VstsClient vstsClient)
        {
            _configuration = configuration;
            _vstsClient = vstsClient;
        }

        public bool PassFilter(MessageAttachment attachment)
        {
            var releaseField = attachment.Fields.FirstOrDefault(_ => _.Title == "Release");

            var releaseName = releaseField?.Value.Replace(">", "").Split('|').Last();
            if (string.IsNullOrWhiteSpace(releaseName)) return false;

            var releaseDefinition = _vstsClient.GetReleaseDefinitionByName(releaseName);
            return releaseDefinition.Name.StartsWith(_configuration.ReleasePrefix);
        }
    }
}