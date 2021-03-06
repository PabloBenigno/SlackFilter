﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SlackFilter.Configuration;
using SlackFilter.ServiceClients.Cache;
using SlackFilter.ServiceClients.Model;


namespace SlackFilter.ServiceClients
{
    public class VstsClient
    {
        private readonly string _vstscredentials;
        private readonly string _vstsBaseAddress;

        public VstsClient(SlackFilterConfiguration configuration)
        {
            _vstscredentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{configuration.PersonalToken}"));
            _vstsBaseAddress = configuration.VstsBaseAddress;
        }

        public BuildDefinition[] GetBuildDefinitionList()
        {
            return GetVstsItems<GetBuildDefinitionResult>("SoderbergPartners/_apis/build/definitions?api-version=5.0-preview.6").Value;
        }

        private T GetVstsItems<T>(string request)
        {
            string result;

            //use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_vstsBaseAddress); //url of our account
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
            var retrieveBuildDefinitionByName = GetVstsItems<GetBuildDefinitionResult>(
                $"SoderbergPartners/_apis/build/definitions?name={name}&api-version=5.0-preview.6");
            return retrieveBuildDefinitionByName.Value.First();
        }
    }
}
