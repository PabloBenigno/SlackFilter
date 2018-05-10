using SlackFilter.Configuration;
using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageTransformers
{
    internal class AttachmentTransformerFactory
    {
        public static IAttachmentTransformer GetAttachmentTransformer(SlackMessageSubject subject, MessageTransformation transformation)
        {
            switch (subject)
            {
                case SlackMessageSubject.BuildCompleted:
                    return new BuildCompletedTransformer(transformation);
                default:
                    return new MessageNoChangeTransformer();
            }
        }
    }
}