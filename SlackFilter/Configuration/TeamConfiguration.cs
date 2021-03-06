﻿namespace SlackFilter.Configuration
{
    public class TeamConfiguration
    {
        public string Name { get; set; }
        public string SlackUrl { get; set; }
        public MessageTransformation MessageTransformation { get; set; }
        public string BuildPath { get; set; }
        public string ReleasePrefix { get; set; }
        public string RepositoryPrefix { get; set; }
    }
}