using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackFilter.Configuration;
using SlackFilter.Model;

namespace SlackFilter.Controllers
{
    public class SlackMessageProcessor
    {
        private readonly ILogger<SlackMessageProcessor> _logger;
        private readonly TeamConfiguration[] _teamConfigurations;

        public SlackMessageProcessor(SlackFilterConfiguration configuration, ILogger<SlackMessageProcessor> logger)
        {
            _teamConfigurations = configuration.TeamConfigurations;

            if (logger != null) _logger = logger;
        }

        public void ProcessMessage(JObject message, SlackMessageSubject subject)
        {
            var slackMessage = JsonConvert.DeserializeObject<SlackMessage>(message.ToString());

            if (slackMessage == null)
            {
                _logger.LogWarning($"Invalid message received {message}");
                return;
            }

            foreach (var teamConfiguration in _teamConfigurations)
            {
                _logger.LogInformation($"Checking filters for team {teamConfiguration.Name}...");
                ProcessMessageByTeam(subject, slackMessage, teamConfiguration);
            }
        }

        private void ProcessMessageByTeam(SlackMessageSubject subject, SlackMessage slackMessage,
            TeamConfiguration teamConfiguration)
        {
            var passingAttachments = slackMessage.Attachments
                .Where(_ => PassFilter(_, subject, teamConfiguration)).ToArray();

            if (passingAttachments.Any())
            {
                var messageToPost = new SlackMessage
                {
                    Attachments = passingAttachments.Select(_ => TransformMessage(_, teamConfiguration.MessageTransformation))
                        .ToArray()
                };

                PostMessageToSlack(JsonConvert.SerializeObject(messageToPost), teamConfiguration.SlackUrl);
            }
            else
                _logger.LogInformation("Failed to pass the filter.");
        }

        private static bool PassFilter(MessageAttachment attachment, SlackMessageSubject subject, TeamConfiguration configuration)
        {
            switch (subject)
            {
                case SlackMessageSubject.BuildCompleted:
                    return attachment.Fields.Any(_ => RequesterIsAllowed(_, configuration.RequesterList))
                           || attachment.Fields.Any(_ => BuildIsAllowed(_, configuration.BuildList));
                case SlackMessageSubject.PullRequestCreated:
                case SlackMessageSubject.ReleaseCompleted:
                    return true;
                default:
                    return false;
            }
        }

        private static MessageAttachment TransformMessage(MessageAttachment attachment, MessageTransformation transformation)
        {
            if (transformation == null) return attachment;

            if (attachment.Pretext.EndsWith("partially succeeded"))
                attachment.Pretext = $"{attachment.Pretext} {transformation.PartialSuccessSuffix}";
            else if (attachment.Pretext.EndsWith("succeeded"))
                attachment.Pretext = $"{attachment.Pretext} {transformation.SuccessSuffix}";
            else if (attachment.Pretext.EndsWith("failed"))
                attachment.Pretext = $"{attachment.Pretext} {transformation.FailSuffix}";
            return attachment;
        }

        private void PostMessageToSlack(string value, string slackUrl)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(
                    slackUrl,
                    new StringContent(value, Encoding.UTF8, "application/json")).Result;
                using (var streamReader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                {
                    var result = streamReader.ReadToEnd();
                    _logger.LogInformation(result);
                }
            }
        }

        private static bool BuildIsAllowed(MessageField field, string[] buildList)
        {
            return field.Title == "Build Definition" && buildList.Contains(field.Value);
        }

        private static bool RequesterIsAllowed(MessageField field, string[] requesterList)
        {
            return field.Title == "Requested by" && requesterList.Contains(field.Value);
        }
    }
}