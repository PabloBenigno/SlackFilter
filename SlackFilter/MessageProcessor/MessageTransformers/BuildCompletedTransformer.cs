using SlackFilter.Configuration;
using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageTransformers
{
    internal class BuildCompletedTransformer : IAttachmentTransformer
    {
        private readonly MessageTransformation _transformation;

        public BuildCompletedTransformer(MessageTransformation transformation)
        {
            _transformation = transformation;
        }

        public MessageAttachment TransformAttachment(MessageAttachment attachment)
        {
            if (_transformation == null) return attachment;

            if (attachment.Pretext.EndsWith("partially succeeded"))
                attachment.Pretext = $"{attachment.Pretext} {_transformation.PartialSuccessSuffix}";
            else if (attachment.Pretext.EndsWith("succeeded"))
                attachment.Pretext = $"{attachment.Pretext} {_transformation.SuccessSuffix}";
            else if (attachment.Pretext.EndsWith("failed"))
                attachment.Pretext = $"{attachment.Pretext} {_transformation.FailSuffix}";
            return attachment;
        }
    }
}