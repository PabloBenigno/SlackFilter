using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackFilter.Model;

namespace SlackFilter.Controllers
{
    [Route("api/[controller]")]
    public class SlackController : Controller
    {
        private readonly ILogger<SlackController> _logger;
        private readonly string _slackUrl;
        private readonly SlackFilterHelper _slackFilterHelper;

        public SlackController(SlackFilterConfiguration configuration, ILogger<SlackController> logger = null)
        {
            _slackUrl = configuration.SlackUrl;
            _slackFilterHelper = new SlackFilterHelper(configuration);
            if (logger != null) _logger = logger;
        }

        [HttpPost]
        [Route("BuildCompleted")]
        public void BuildCompleted([FromBody] JObject message)
        {
            _logger.LogInformation($"Build completed:\n {message}!");

            ProcessMessage(message, SlackMessageSubject.BuildCompleted);
        }

        [HttpPost]
        [Route("PullRequestCreated")]
        public void PullRequestCreated([FromBody] JObject message)
        {
            _logger.LogInformation($"Pull request created:\n {message}!");

            ProcessMessage(message, SlackMessageSubject.PullRequestCreated);
        }

        [HttpPost]
        [Route("ReleaseCompleted")]
        public void ReleaseCompleted([FromBody] JObject message)
        {
            _logger.LogInformation($"Release completed:\n {message}!");
            ProcessMessage(message, SlackMessageSubject.ReleaseCompleted);
        }

        private void ProcessMessage(JObject message, SlackMessageSubject subject)
        {
            var slackMessage = JsonConvert.DeserializeObject<SlackMessage>(message.ToString());

            var passingAttachments = slackMessage.Attachments
                .Where(_ => _slackFilterHelper.PassFilter(_, subject)).ToList();

            if (passingAttachments.Any())
            {
                var messageToPost = new SlackMessage
                {
                    Attachments = passingAttachments.Select(m => _slackFilterHelper.TransformMessage(m)).ToArray()
                };

                PostMessageToSlack(JsonConvert.SerializeObject(messageToPost));
            }
            else
                _logger.LogInformation("Failed to pass the filter.");
        }

        private void PostMessageToSlack(string value)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(
                    _slackUrl,
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
