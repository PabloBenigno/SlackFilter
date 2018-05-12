using System.Linq;
using SlackFilter.Configuration;
using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal class PullRequestCreatedFilter : IAttachmentFilter
    {
        private readonly TeamConfiguration _configuration;

        public PullRequestCreatedFilter(TeamConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool PassFilter(MessageAttachment attachment)
        {
            var repositoryName = attachment.Pretext.Replace(">", "").Split('|').Last();
            return !string.IsNullOrWhiteSpace(repositoryName) &&
                   repositoryName.StartsWith(_configuration.RepositoryPrefix);
        }
    }
}