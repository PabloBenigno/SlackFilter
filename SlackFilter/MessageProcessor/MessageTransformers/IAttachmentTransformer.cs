using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageTransformers
{
    internal interface IAttachmentTransformer
    {
        MessageAttachment TransformAttachment(MessageAttachment attachment);
    }
}