using System.Linq;
using SlackFilter.Model;

namespace SlackFilter.Controllers
{
    public class SlackFilterHelper
    {
        private readonly string[] _requesterList;
        private readonly string[] _buildList;
        private readonly string _successSuffix;
        private readonly string _partialSuccessSuffix;
        private readonly string _failSuffix;

        public SlackFilterHelper(SlackFilterConfiguration configuration)
        {
            _requesterList = configuration.RequesterList.Split(",").Select(_ => _.Trim()).ToArray();
            _buildList = configuration.BuildList.Split(",").Select(_ => _.Trim()).ToArray();
            _successSuffix = configuration.SuccessSuffix;
            _partialSuccessSuffix = configuration.PartialSuccessSuffix;
            _failSuffix = configuration.FailSuffix;
        }

        public bool PassFilter(MessageAttachment attachment, SlackMessageSubject subject)
        {
            switch (subject)
            {
                case SlackMessageSubject.BuildCompleted:
                    return attachment.Fields.Any(RequesterIsAllowed)
                           || attachment.Fields.Any(BuildIsAllowed);
                case SlackMessageSubject.PullRequestCreated:
                case SlackMessageSubject.ReleaseCompleted:
                    return true;
                default:
                    return false;
            }
        }

        public MessageAttachment TransformMessage(MessageAttachment attachment)
        {
            if (attachment.Pretext.EndsWith("partially succeeded"))
                attachment.Pretext = $"{attachment.Pretext} {_partialSuccessSuffix}";
            else if (attachment.Pretext.EndsWith("succeeded"))
                attachment.Pretext = $"{attachment.Pretext} {_successSuffix}";
            else if (attachment.Pretext.EndsWith("failed"))
                attachment.Pretext = $"{attachment.Pretext} {_failSuffix}";
            return attachment;
        }

        private bool BuildIsAllowed(MessageField field)
        {
            return field.Title == "Build Definition" && _buildList.Contains(field.Value);
        }

        private bool RequesterIsAllowed(MessageField field)
        {
            return field.Title == "Requested by" && _requesterList.Contains(field.Value);
        }
    }
}