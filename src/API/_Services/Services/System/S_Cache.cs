using System.Text;
using System.Text.Json;
using API._Services.Interfaces.System;
using Microsoft.Extensions.Caching.Distributed;

namespace API._Services.Services.System;
public class S_Cache(IDistributedCache distributedCache, IConfiguration configuration) : I_Cache
{
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly IConfiguration _configuration = configuration;

    public async Task<T?> GetAsync<T>(string key)
    {
        byte[]? cacheData = await _distributedCache.GetAsync(key);
        if (cacheData is not null)
        {
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(cacheData));
        }

        return default;
    }

    public async Task SetAsync<T>(string key, T value, int timeDurationInHours = 0)
    {
        byte[] byteValue = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        if (timeDurationInHours == 0)
        {
            timeDurationInHours = _configuration.GetValue<int>("CacheDurationInHours");
        }

        await _distributedCache.SetAsync(key, byteValue, new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(timeDurationInHours)));
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }
}
