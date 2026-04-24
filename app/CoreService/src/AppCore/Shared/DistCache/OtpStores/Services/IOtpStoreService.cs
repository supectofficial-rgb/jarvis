namespace Insurance.AppCore.Shared.OtpStores.Services;

using System;
using System.Collections.Generic;
using System.Text;

public interface IOtpStoreService
{
    Task SetAsync(string key, string code, TimeSpan ttl);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
}