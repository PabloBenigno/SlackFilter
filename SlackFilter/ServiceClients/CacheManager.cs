using System;
using Microsoft.Extensions.Caching.Memory;

namespace SlackFilter.ServiceClients
{
    internal static class CacheManager
    {
        private static readonly MemoryCache BuildDefinitionsCache = new MemoryCache(new MemoryCacheOptions());
        private static readonly MemoryCache ReleaseDefinitionCache = new MemoryCache(new MemoryCacheOptions());

        public static BuildDefinition GetOrAddItemIntoBuildDefinitionCache(string key, Func<BuildDefinition> valueFactory)
        {
            return GetOrAddItemIntoCache(key, BuildDefinitionsCache, valueFactory);
        }

        public static ReleaseDefinition GetOrAddItemIntoReleaseDefinitionCache(string key, Func<ReleaseDefinition> valueFactory)
        {
            return GetOrAddItemIntoCache(key, ReleaseDefinitionCache, valueFactory);
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

        public static void InitializeReleaseDefinitionList(ReleaseDefinition[] releaseDefinitionList)
        {
            foreach (var releaseDefinition in releaseDefinitionList)
                ReleaseDefinitionCache.Set(releaseDefinition.Name, releaseDefinition);
        }
    }
}