using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SlackFilter.Configuration;


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
            return CacheManager.GetOrAddItemIntoBuildDefinitionCache(name, () => RetrieveBuildDefinitionByName(name));
        }

        private BuildDefinition RetrieveBuildDefinitionByName(string name)
        {
            var retrieveBuildDefinitionByName = GetVstsItems<GetBuildDefinitionResult>($"SoderbergPartners/_apis/build/definitions?name={name}&api-version=5.0-preview.6");
            return retrieveBuildDefinitionByName.Value.First();
        }
    }

    internal static class CacheManager
    {
        private static readonly MemoryCache BuildDefinitionsCache = new MemoryCache(new MemoryCacheOptions());

        internal static MemoryCacheOptions DefaultShortCacheItemPolicy { get; set; }

        public static T GetOrAddItemIntoBuildDefinitionCache<T>(string key, Func<T> valueFactory) where T : BuildDefinition
        {
            return GetOrAddItemIntoCache(key, BuildDefinitionsCache, valueFactory);
        }

        private static T GetOrAddItemIntoCache<T>(string key, MemoryCache cache, Func<T> valueFactory) where T : BuildDefinition
        {
            return cache.AddOrGetExisting<T>(key, valueFactory);
        }

        public static void InitializeBuildDefinitionList(BuildDefinition[] buildDefinitionList)
        {
            foreach (var buildDefinition in buildDefinitionList)
                BuildDefinitionsCache.Set(buildDefinition.Name, buildDefinition);
        }
    }

    internal static class MemoryCacheExtensions
    {
        public static T AddOrGetExisting<T>(this MemoryCache cache, string key, Func<T> valueFactory)
        {
            try
            {
                if (cache.TryGetValue(key, out T oldValue))
                    return oldValue;

                var newValue = valueFactory.Invoke();
                cache.Set(key, newValue);
                return newValue;
            }
            catch
            {
                cache.Remove(key);
                throw;
            }
        }
    }
        
    

    public class GetBuildDefinitionResult
    {
        public int Count { get; set; }
        public BuildDefinition[] Value { get; set; }
    }

    public class BuildDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string QueueStatus { get; set; }
    }
}
