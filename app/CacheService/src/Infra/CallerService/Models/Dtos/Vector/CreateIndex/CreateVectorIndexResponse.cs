namespace Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.CreateIndex;

public sealed record CreateVectorIndexResponse(bool Created, bool AlreadyExists, string Message);
