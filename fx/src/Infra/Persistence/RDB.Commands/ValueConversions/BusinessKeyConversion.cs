namespace OysterFx.Infra.Persistence.RDB.Commands.ValueConversions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OysterFx.AppCore.Domain.ValueObjects;

public class BusinessKeyConversion : ValueConverter<BusinessKey, Guid>
{
    public BusinessKeyConversion() : base(c => c.Value, c => c) { }
}