using System;
using System.Collections.Generic;
using Domain.Models.Users;
using Microsoft.Extensions.Caching.Memory;

namespace Services
{
    public class LocalCacheService
    {
        private readonly IMemoryCache _cache;

        public LocalCacheService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public List<T> GetList<T>(Guid key)
        {
            return _cache.Get<List<T>>(key);
        }

        public void SetList<T>(Guid key, List<T> value, TimeSpan expirationTime)
        {
            _cache.Set(key, value, expirationTime);
        }

        public virtual bool CanView(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return true;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return true;
        }
    }
}