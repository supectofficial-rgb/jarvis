namespace Insurance.AppCore.Shared.TokenVersionStores.Services;

using System;
using System.Collections.Generic;
using System.Text;

public interface ITokenVersionStore
{
    Task<int> GetVersionAsync(long userId);
    Task IncrementAsync(long userId);
}