using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageTransformers
{
    internal class MessageNoChangeTransformer : IAttachmentTransformer
    {
        public MessageAttachment TransformAttachment(MessageAttachment attachment)
        {
            return attachment;
        }
    }
}