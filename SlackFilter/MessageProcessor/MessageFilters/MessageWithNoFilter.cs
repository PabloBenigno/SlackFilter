using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal class MessageWithNoFilter : IAttachmentFilter
    {
        public bool PassFilter(MessageAttachment attachment)
        {
            return true;
        }
    }
}