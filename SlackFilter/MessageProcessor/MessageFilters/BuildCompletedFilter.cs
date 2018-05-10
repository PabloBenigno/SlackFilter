using System.Linq;
using SlackFilter.Configuration;
using SlackFilter.Model;
using static SlackFilter.MessageProcessor.FieldPredicates;

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
            return attachment.Fields.Any(_ => RequesterIsAllowed(_, _configuration.RequesterList))
                   || attachment.Fields.Any(_ => BuildIsAllowed(_, _configuration.BuildList));
        }
    }
}