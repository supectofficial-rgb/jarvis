namespace Insurance.AppCore.Shared.DistCache.RateLimitStores;

using System;
using System.Collections.Generic;
using System.Text;

public interface IRateLimitStore
{
    Task<bool> IsAllowedAsync(string key, int limit, TimeSpan window);
}