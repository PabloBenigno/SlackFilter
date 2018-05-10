using System.Linq;
using SlackFilter.Configuration;
using SlackFilter.MessageProcessor;
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
            return _configuration.RequesterList.Any(_ => attachment.Pretext.StartsWith((string) _)) ||
                   attachment.Fields.Any(_ => FieldPredicates.ReviewersAreAllowed(_, _configuration.RequesterList));
        }
    }
}