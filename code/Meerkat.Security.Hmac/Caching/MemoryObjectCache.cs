using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Meerkat.Caching
{
    /// <summary>
    /// Simple <see cref="MemoryCache"/> wrapper for <see cref="ICache"/>
    /// </summary>
    public class MemoryObjectCache : ICache
    {
        public object this[string key]
        {
            get { return MemoryCache.Default[key]; }
            set { MemoryCache.Default[key] = value; }
        }

        public long CacheMemoryLimit
        {
            get { return MemoryCache.Default.CacheMemoryLimit; }
        }

        public string Name
        {
            get { return MemoryCache.Default.Name; }
        }

        public object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            return MemoryCache.Default.AddOrGetExisting(key, value, absoluteExpiration, regionName);
        }

        public bool Contains(string key, string regionName = null)
        {
            return MemoryCache.Default.Contains(key, regionName);
        }

        public object Get(string key, string regionName = null)
        {
            return MemoryCache.Default.Get(key, regionName);
        }

        public long GetCount(string regionName = null)
        {
            return MemoryCache.Default.GetCount(regionName);
        }

        public IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            return MemoryCache.Default.GetValues(keys, regionName);
        }

        public object Remove(string key, string regionName = null)
        {
            return MemoryCache.Default.Remove(key, regionName);
        }

        public void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            MemoryCache.Default.Set(key, value, absoluteExpiration, regionName);
        }
    }
}