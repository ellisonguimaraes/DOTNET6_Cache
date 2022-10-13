using Microsoft.Extensions.Caching.Memory;

namespace RedisCache.API.Caching;

public class MemoryCachingService : ICachingService
{
    private readonly IMemoryCache _memoryCache;

    private readonly MemoryCacheEntryOptions _options;

    public MemoryCachingService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _options = new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
            SlidingExpiration = TimeSpan.FromSeconds(1200)
        };
    }

    public Task<T> GetAsync<T>(string key)
    {
        var value = _memoryCache.Get<T>(key);
        return Task.FromResult<T>(value);
    }

    public Task SetAsync<T>(string key, T value)
    {
        _memoryCache.Set<T>(key, value, _options);
        return Task.CompletedTask;
    }
}
