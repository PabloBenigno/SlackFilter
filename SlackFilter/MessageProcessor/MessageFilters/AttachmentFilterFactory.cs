using SlackFilter.Configuration;
using SlackFilter.Model;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal class AttachmentFilterFactory
    {
        public static IAttachmentFilter GetAttachmentFilter(SlackMessageSubject subject, TeamConfiguration configuration)
        {
            switch (subject)
            {
                case SlackMessageSubject.BuildCompleted:
                    return new BuildCompletedFilter(configuration);
                case SlackMessageSubject.PullRequestCreated:
                    return new PullRequestCreatedFilter(configuration);
                case SlackMessageSubject.ReleaseCompleted:
                    return new ReleaseCompletedFilter(configuration);
                default:
                    return new MessageWithNoFilter();
            }
        }
    }
}