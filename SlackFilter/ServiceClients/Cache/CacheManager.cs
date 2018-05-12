using System;
using Microsoft.Extensions.Caching.Memory;
using SlackFilter.ServiceClients.Model;

namespace SlackFilter.ServiceClients.Cache
{
    internal static class CacheManager
    {
        private static readonly MemoryCache BuildDefinitionsCache = new MemoryCache(new MemoryCacheOptions());

        public static BuildDefinition GetOrAddItemIntoBuildDefinitionCache(string key, Func<BuildDefinition> valueFactory)
        {
            return GetOrAddItemIntoCache(key, BuildDefinitionsCache, valueFactory);
        }

        private static T GetOrAddItemIntoCache<T>(string key, MemoryCache cache, Func<T> valueFactory)
        {
            return cache.AddOrGetExisting(key, valueFactory);
        }

        public static void InitializeBuildDefinitionList(BuildDefinition[] buildDefinitionList)
        {
            foreach (var buildDefinition in buildDefinitionList)
                BuildDefinitionsCache.Set(buildDefinition.Name, buildDefinition);
        }
    }
}