namespace Insurance.CacheService.AppCore.Shared.Vector.Services;

public sealed record VectorIndexCreateResult(bool Created, bool AlreadyExists, string Message);
