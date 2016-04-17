using System;
using System.Collections.Generic;

namespace Meerkat.Caching
{
    /// <summary>
    /// Provides object caching
    /// </summary>
    public interface ICache
    {
        //
        // Summary:
        //     Gets or sets a value in the cache by using the default indexer property for an
        //     instance of the System.Runtime.Caching.MemoryCache class.
        //
        // Parameters:
        //   key:
        //     A unique identifier for the cache value to get or set.
        //
        // Returns:
        //     The value in the cache instance for the specified key, if the entry exists; otherwise,
        //     null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     key is null. -or-The inserted value is null.
        object this[string key] { get; set; }

        //
        // Summary:
        //     Gets the amount of memory on the computer, in bytes, that can be used by the
        //     cache.
        //
        // Returns:
        //     The amount of memory in bytes.
        long CacheMemoryLimit { get; }
        //
        // Summary:
        //     Gets the name of the cache.
        //
        // Returns:
        //     The name of the cache.
        string Name { get; }

        //
        // Summary:
        //     Adds a cache entry into the cache using the specified key and a value and an
        //     absolute expiration value.
        //
        // Parameters:
        //   key:
        //     A unique identifier for the cache entry to add.
        //
        //   value:
        //     The data for the cache entry.
        //
        //   absoluteExpiration:
        //     The fixed date and time at which the cache entry will expire.
        //
        //   regionName:
        //     A named region in the cache to which a cache entry can be added. Do not pass
        //     a value for this parameter. This parameter is null by default, because the System.Runtime.Caching.MemoryCache
        //     class does not implement regions.
        //
        // Returns:
        //     If a cache entry with the same key exists, the existing cache entry; otherwise,
        //     null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     key is null.
        //
        //   T:System.ArgumentNullException:
        //     value is not null.
        //
        //   T:System.NotSupportedException:
        //     regionName is not null.
        //
        //   T:System.ArgumentException:
        //     An System.Runtime.Caching.CacheItemPolicy.UpdateCallback property has been supplied.
        //     NoteThe Overload:System.Runtime.Caching.ObjectCache.Add and the Overload:System.Runtime.Caching.ObjectCache.AddOrGetExisting
        //     method overloads do not support the System.Runtime.Caching.CacheItemPolicy.UpdateCallback
        //     property. Therefore, to set the System.Runtime.Caching.CacheItemPolicy.UpdateCallback
        //     property for a cache entry, use the Overload:System.Runtime.Caching.MemoryCache.Set
        //     overloads instead.
        //
        //   T:System.ArgumentException:
        //     Both the absolute and sliding expiration values for the System.Runtime.Caching.CacheItemPolicy
        //     object are set to values other than the defaults of System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration
        //     and System.Runtime.Caching.ObjectCache.NoSlidingExpiration fields. The System.Runtime.Caching.MemoryCache
        //     class cannot set expiration policy based on a combination of an absolute expiration
        //     and a sliding expiration. Only one expiration setting can be explicitly set when
        //     you use the System.Runtime.Caching.MemoryCache instance. The other expiration
        //     setting must be set to System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration
        //     or System.Runtime.Caching.ObjectCache.NoSlidingExpiration
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The System.Runtime.Caching.CacheItemPolicy.SlidingExpiration property is set
        //     to a value less than System.TimeSpan.Zero. -or-The System.Runtime.Caching.CacheItemPolicy.SlidingExpiration
        //     property is set to a value greater than one year.-or-The System.Runtime.Caching.CacheItemPolicy.Priority
        //     property is not a value of the System.Runtime.Caching.CacheItemPriority enumeration.
        object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null);

        //
        // Summary:
        //     Determines whether a cache entry exists in the cache.
        //
        // Parameters:
        //   key:
        //     A unique identifier for the cache entry to search for.
        //
        //   regionName:
        //     A named region in the cache to which a cache entry was added. Do not pass a value
        //     for this parameter. This parameter is null by default, because the System.Runtime.Caching.MemoryCache
        //     class does not implement regions.
        //
        // Returns:
        //     true if the cache contains a cache entry whose key matches key; otherwise, false.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     key is null.
        //
        //   T:System.NotSupportedException:
        //     regionName is not null.
        bool Contains(string key, string regionName = null);

        //
        // Summary:
        //     Returns an entry from the cache.
        //
        // Parameters:
        //   key:
        //     A unique identifier for the cache entry to get.
        //
        //   regionName:
        //     A named region in the cache to which a cache entry was added. Do not pass a value
        //     for this parameter. This parameter is null by default, because the System.Runtime.Caching.MemoryCache
        //     class does not implement regions.
        //
        // Returns:
        //     A reference to the cache entry that is identified by key, if the entry exists;
        //     otherwise, null.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     regionName is not null.
        //
        //   T:System.ArgumentNullException:
        //     key is null.
        object Get(string key, string regionName = null);

        //
        // Summary:
        //     Returns the total number of cache entries in the cache.
        //
        // Parameters:
        //   regionName:
        //     A named region in the cache to which a cache entry was added. Do not pass a value
        //     for this parameter. This parameter is null by default, because the System.Runtime.Caching.MemoryCache
        //     class does not implement regions.
        //
        // Returns:
        //     The number of entries in the cache.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     regionName is not null.
        long GetCount(string regionName = null);

        //
        // Summary:
        //     Returns a set of cache entries that correspond to the specified keys.
        //
        // Parameters:
        //   keys:
        //     A set of unique identifiers for the cache entries to return.
        //
        //   regionName:
        //     A named region in the cache to which a cache entry was added. Do not pass a value
        //     for this parameter. This parameter is null by default, because the System.Runtime.Caching.MemoryCache
        //     class does not implement regions.
        //
        // Returns:
        //     A set of cache entries that correspond to the specified keys.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     regionName is not null.
        //
        //   T:System.ArgumentNullException:
        //     keys is null.
        //
        //   T:System.ArgumentException:
        //     An individual key in the collection is null.
        IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null);

        //
        // Summary:
        //     Removes a cache entry from the cache.
        //
        // Parameters:
        //   key:
        //     A unique identifier for the cache entry to remove.
        //
        //   regionName:
        //     A named region in the cache to which a cache entry was added. Do not pass a value
        //     for this parameter. This parameter is null by default, because the System.Runtime.Caching.MemoryCache
        //     class does not implement regions.
        //
        // Returns:
        //     If the entry is found in the cache, the removed cache entry; otherwise, null.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     regionName is not null.
        //
        //   T:System.ArgumentNullException:
        //     key is null.
        object Remove(string key, string regionName = null);

        //
        // Summary:
        //     Inserts a cache entry into the cache by using a key and a value and specifies
        //     time-based expiration details.
        //
        // Parameters:
        //   key:
        //     A unique identifier for the cache entry to insert.
        //
        //   value:
        //     The data for the cache entry.
        //
        //   absoluteExpiration:
        //     The fixed date and time at which the cache entry will expire.
        //
        //   regionName:
        //     A named region in the cache to which a cache entry can be added. Do not pass
        //     a value for this parameter. This parameter is null by default, because the System.Runtime.Caching.MemoryCache
        //     class does not implement regions.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     regionName is not null.
        //
        //   T:System.ArgumentNullException:
        //     key is null.-or-Value is null.
        //
        //   T:System.ArgumentException:
        //     An invalid combination of arguments for the cache entry was passed. This occurs
        //     if the following expiration details are set on the policy object for the cache
        //     entry:If both the absolute and sliding expiration values on System.Runtime.Caching.CacheItemPolicy
        //     object are set to values other than the defaults of System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration
        //     and System.Runtime.Caching.ObjectCache.NoSlidingExpiration. This occurs because
        //     the System.Runtime.Caching.MemoryCache class does not support expiring entries
        //     based on both an absolute and a sliding expiration. Only one expiration setting
        //     can be explicitly set when you use the System.Runtime.Caching.MemoryCache class.
        //     The other setting must be set to System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration
        //     or System.Runtime.Caching.ObjectCache.NoSlidingExpiration. If both the removal
        //     callback and the update callback are specified on System.Runtime.Caching.CacheItemPolicy
        //     object. The System.Runtime.Caching.MemoryCache class only supports using one
        //     type of callback per cache entry.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The System.Runtime.Caching.CacheItemPolicy.SlidingExpiration property is set
        //     to a value less than System.TimeSpan.Zero. -or-The System.Runtime.Caching.CacheItemPolicy.SlidingExpiration
        //     property is set to a value greater than one year.-or-The System.Runtime.Caching.CacheItemPolicy.Priority
        //     property is not a value of the System.Runtime.Caching.CacheItemPriority enumeration.
        void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null);
    }
}
