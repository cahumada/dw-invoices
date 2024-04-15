using Microsoft.Extensions.Caching.Memory;

namespace iaas.app.dw.invoices.Application.Base
{
    public class MemoryCacher
    {
        private readonly IMemoryCache _cache;

        public MemoryCacher(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<IMemoryCache> Get()
        {
            var memoryCacheAux = _cache;
            return memoryCacheAux;
        }

        public async Task<T> GetValue<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public async Task<T> Add<T>(string key, T value, DateTimeOffset expiration)
        {
            return _cache.Set(key, value, expiration);
        }

        public async Task<bool> Delete(string key)
        {
            if (_cache.Get(key) != null)
                _cache.Remove(key);

            return true;
        }
    }
}
