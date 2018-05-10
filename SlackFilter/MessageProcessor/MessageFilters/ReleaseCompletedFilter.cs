using System.Linq;
using SlackFilter.Configuration;
using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal class ReleaseCompletedFilter : IAttachmentFilter
    {
        private readonly TeamConfiguration _configuration;

        public ReleaseCompletedFilter(TeamConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool PassFilter(MessageAttachment attachment)
        {
            return _configuration.ReleaseList.Any(_ =>
                attachment.Fallback.StartsWith($"Deployment of release {_}"));
        }
    }
}