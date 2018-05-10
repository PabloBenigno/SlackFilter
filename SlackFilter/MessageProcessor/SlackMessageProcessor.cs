using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackFilter.Configuration;
using SlackFilter.MessageProcessor.MessageFilters;
using SlackFilter.MessageProcessor.MessageTransformers;
using SlackFilter.Model;

namespace SlackFilter.MessageProcessor
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
                    Attachments = passingAttachments
                        .Select(_ => TransformMessage(_, subject, teamConfiguration.MessageTransformation))
                        .ToArray()
                };

                PostMessageToSlack(JsonConvert.SerializeObject(messageToPost), teamConfiguration.SlackUrl);
            }
            else
                _logger.LogInformation("Failed to pass the filter.");
        }

        private static bool PassFilter(MessageAttachment attachment, SlackMessageSubject subject, TeamConfiguration configuration)
        {
            var attachmentFilter = AttachmentFilterFactory.GetAttachmentFilter(subject, configuration);
            return attachmentFilter != null && attachmentFilter.PassFilter(attachment);
        }

        private static MessageAttachment TransformMessage(MessageAttachment attachment, SlackMessageSubject subject, MessageTransformation transformation)
        {
            var attachmentTransformer = AttachmentTransformerFactory.GetAttachmentTransformer(subject, transformation);
            return attachmentTransformer.TransformAttachment(attachment);
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
    }
}