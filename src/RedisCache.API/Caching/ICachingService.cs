namespace RedisCache.API.Caching;

public interface ICachingService
{
    Task<T> GetAsync<T>(string key);

    Task SetAsync<T>(string key, T value);
}