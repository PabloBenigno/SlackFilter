﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackFilter.MessageProcessor;
using SlackFilter.Model;
using SlackFilter.ServiceClients;
using SlackFilter.ServiceClients.Cache;
using Spin.Logger.Abstractions;

namespace SlackFilter.Controllers
{
    [Route("api/[controller]")]
    public class SlackController : Controller
    {
        private readonly ISpinLogger<SlackController> _logger;
        private readonly VstsClient _vstsClient;
        private readonly SlackMessageProcessor _slackMessageProcessor;
        
        public SlackController(VstsClient vstsClient, SlackMessageProcessor slackFilterHelper, ISpinLogger<SlackController> logger = null)
        {
            _vstsClient = vstsClient;
            _slackMessageProcessor = slackFilterHelper;
            if (logger != null) _logger = logger;

            
            CacheManager.InitializeBuildDefinitionList(_vstsClient.GetBuildDefinitionList());
        }
        
        [HttpPost]
        [Route("BuildCompleted")]
        public void BuildCompleted([FromBody] JObject message)
        {
            _logger.Log(LogLevel.Information, $"Build completed:\n {message}!");

            _slackMessageProcessor.ProcessMessage(message, SlackMessageSubject.BuildCompleted);
        }

        [HttpPost]
        [Route("PullRequestCreated")]
        public void PullRequestCreated([FromBody] JObject message)
        {
            _logger.Log(LogLevel.Information, $"Pull request created:\n {message}!");

            _slackMessageProcessor.ProcessMessage(message, SlackMessageSubject.PullRequestCreated);
        }

        [HttpPost]
        [Route("ReleaseCompleted")]
        public void ReleaseCompleted([FromBody] JObject message)
        {
            _logger.Log(LogLevel.Information, $"Release completed:\n {message}!");
            _slackMessageProcessor.ProcessMessage(message, SlackMessageSubject.ReleaseCompleted);
        }
    }
}
