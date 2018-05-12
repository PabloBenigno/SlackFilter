using System;
using Microsoft.Extensions.Caching.Memory;

namespace SlackFilter.ServiceClients.Cache
{
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
}