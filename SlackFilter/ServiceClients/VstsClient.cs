using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SlackFilter.Configuration;


namespace SlackFilter.ServiceClients
{
    public class VstsClient
    {
        private readonly string _vstscredentials;
        private readonly string _vsrmBaseAddress;
        private readonly string _vstsBaseAddress;

        public VstsClient(SlackFilterConfiguration configuration)
        {
            _vstscredentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{configuration.PersonalToken}"));
            _vstsBaseAddress = configuration.VstsBaseAddress;
            _vsrmBaseAddress = configuration.VsrmBaseAddress;
        }

        public BuildDefinition[] GetBuildDefinitionList()
        {
            return GetVstsItems<GetBuildDefinitionResult>(_vstsBaseAddress, "SoderbergPartners/_apis/build/definitions?api-version=5.0-preview.6").Value;
        }

        public ReleaseDefinition[] GetReleaseDefinitionList()
        {
            return GetVstsItems<GetReleaseDefinitionResult>(_vsrmBaseAddress, "SoderbergPartners/_apis/release/definitions?api-version=5.0-preview.3").Value;
        }

        private T GetVstsItems<T>(string baseAddress, string request)
        {
            string result;

            //use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress); //url of our account
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _vstscredentials);

                //connect to the REST endpoint            
                var response = client.GetAsync(request).Result;

                result = response.Content.ReadAsStringAsync().Result;
            }

            return JsonConvert.DeserializeObject<T>(result);
        }

        public BuildDefinition GetBuildDefinitionByName(string name)
        {
            return CacheManager.GetOrAddItemIntoBuildDefinitionCache(name, 
                () => RetrieveBuildDefinitionByName(name));
        }

        private BuildDefinition RetrieveBuildDefinitionByName(string name)
        {
            var retrieveBuildDefinitionByName = GetVstsItems<GetBuildDefinitionResult>(_vstsBaseAddress,
                $"SoderbergPartners/_apis/build/definitions?name={name}&api-version=5.0-preview.6");
            return retrieveBuildDefinitionByName.Value.First();
        }

        public ReleaseDefinition GetReleaseDefinitionByName(string name)
        {
            return CacheManager.GetOrAddItemIntoReleaseDefinitionCache(name,
                () => RetrieveReleaseDefinitionByName(name));
        }

        private ReleaseDefinition RetrieveReleaseDefinitionByName(string name)
        {
            var retrieveReleaseDefinitionByName = GetVstsItems<GetReleaseDefinitionResult>(_vsrmBaseAddress,
                $"/SoderbergPartners/_apis/release/definitions?searchText={name}&api-version=5.0-preview.3");
            return retrieveReleaseDefinitionByName.Value.First();
        }
    }
}
