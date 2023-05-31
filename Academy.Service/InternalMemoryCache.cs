using Microsoft.Extensions.Caching.Memory;

namespace Academy.Service;

public class InternalMemoryCache
{
    public MemoryCache Cache { get; } = new MemoryCache(
        new MemoryCacheOptions
        {
            SizeLimit = 1024
        });
}