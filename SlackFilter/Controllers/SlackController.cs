using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackFilter.Model.BuildCompleted;

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

            var buildCompletedMessage = JsonConvert.DeserializeObject<SlackMessage>(message.ToString());

            if (_slackFilterHelper.PassFilter(buildCompletedMessage))
            {
                _slackFilterHelper.TransformMessage(buildCompletedMessage);
                PostMessageToSlack(JsonConvert.SerializeObject(buildCompletedMessage));
            }
            else
                _logger.LogInformation("Failed to pass the filter.");
        }

        [HttpPost]
        [Route("PullRequestCreated")]
        public void PullRequestCreated([FromBody] JObject message)
        {
            _logger.LogInformation($"Pull request created:\n {message}!");
            PostMessageToSlack(message.ToString());
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
