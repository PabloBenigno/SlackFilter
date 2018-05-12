using SlackFilter.Configuration;
using SlackFilter.Model;
using SlackFilter.ServiceClients;

namespace SlackFilter.MessageProcessor.MessageFilters
{
    internal static class AttachmentFilterFactory
    {
        public static IAttachmentFilter GetAttachmentFilter(SlackMessageSubject subject, TeamConfiguration teamConfiguration, SlackFilterConfiguration configuration)
        {
            switch (subject)
            {
                case SlackMessageSubject.BuildCompleted:
                    return new BuildCompletedFilter(teamConfiguration, new VstsClient(configuration));
                case SlackMessageSubject.PullRequestCreated:
                    return new PullRequestCreatedFilter(teamConfiguration);
                case SlackMessageSubject.ReleaseCompleted:
                    return new ReleaseCompletedFilter(teamConfiguration, new VstsClient(configuration));
                default:
                    return new MessageWithNoFilter();
            }
        }
    }
}