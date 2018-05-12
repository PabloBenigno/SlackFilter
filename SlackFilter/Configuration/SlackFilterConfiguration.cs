using Microsoft.Extensions.Configuration.UserSecrets;

namespace SlackFilter.Configuration
{
    public class SlackFilterConfiguration
    {
        public string PersonalToken { get; set; }
        public string VstsBaseAddress { get; set; }
        public TeamConfiguration[] TeamConfigurations { get; set; }
    }
}