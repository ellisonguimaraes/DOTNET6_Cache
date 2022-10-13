using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisCache.API.Caching;

public class RedisCachingService : ICachingService
{
    private readonly IDistributedCache _cache;

    private readonly DistributedCacheEntryOptions _options;

    public RedisCachingService(IDistributedCache cache)
    {
        _cache = cache;    
        _options = new DistributedCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
            SlidingExpiration = TimeSpan.FromSeconds(1200)
        };
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _cache.GetStringAsync(key);

        if (value is null)
            return default(T)!;

        return JsonSerializer.Deserialize<T>(value)!;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var serializeObj = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializeObj, _options);
    }
}