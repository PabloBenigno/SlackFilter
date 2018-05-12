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
using Spin.Logger.Abstractions;

namespace SlackFilter.MessageProcessor
{
    public class SlackMessageProcessor
    {
        private static SlackFilterConfiguration _configuration;
        private readonly ISpinLogger<SlackMessageProcessor> _logger;

        public SlackMessageProcessor(SlackFilterConfiguration configuration, ISpinLogger<SlackMessageProcessor> logger)
        {
            _configuration = configuration;
            if (logger != null) _logger = logger;
        }

        public void ProcessMessage(JObject message, SlackMessageSubject subject)
        {
            var slackMessage = JsonConvert.DeserializeObject<SlackMessage>(message.ToString());

            if (slackMessage == null)
            {
                _logger.Log(LogLevel.Warning, $"Invalid message received {message}");
                return;
            }

            foreach (var teamConfiguration in _configuration.TeamConfigurations)
            {
                _logger.Log(LogLevel.Information, $"Checking filters for team {teamConfiguration.Name}...");
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
                _logger.Log(LogLevel.Information, "Failed to pass the filter.");
        }

        private static bool PassFilter(MessageAttachment attachment, SlackMessageSubject subject, TeamConfiguration configuration)
        {
            var attachmentFilter = AttachmentFilterFactory.GetAttachmentFilter(subject, configuration, _configuration);
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
                    _logger.Log(LogLevel.Information, result);
                }
            }
        }
    }
}