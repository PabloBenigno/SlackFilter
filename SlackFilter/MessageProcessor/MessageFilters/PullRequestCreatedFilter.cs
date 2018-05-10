using System.Linq;
using SlackFilter.Configuration;
using SlackFilter.Model;
using static SlackFilter.MessageProcessor.FieldPredicates;

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
            return _configuration.RequesterList.Any(_ => attachment.Pretext.StartsWith(_)) ||
                   attachment.Fields.Any(_ => ReviewersAreAllowed(_, _configuration.RequesterList));
        }
    }
}