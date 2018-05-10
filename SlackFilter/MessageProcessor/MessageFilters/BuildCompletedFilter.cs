using System.Linq;
using SlackFilter.Configuration;
using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal class BuildCompletedFilter : IAttachmentFilter
    {
        private readonly TeamConfiguration _configuration;

        public BuildCompletedFilter(TeamConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool PassFilter(MessageAttachment attachment)
        {
            return attachment.Fields.Any(_ => FieldPredicates.RequesterIsAllowed(_, _configuration.RequesterList))
                   || attachment.Fields.Any(_ => FieldPredicates.BuildIsAllowed(_, _configuration.BuildList));
        }
    }
}