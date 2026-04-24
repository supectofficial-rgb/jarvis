namespace Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.CreateIndex;

public sealed record CreateVectorIndexResponse(bool Created, bool AlreadyExists, string Message);
