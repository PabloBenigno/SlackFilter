using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal interface IAttachmentFilter
    {
        bool PassFilter(MessageAttachment attachment);
    }
}