namespace OysterFx.Infra.Persistence.RDB.Commands.ValueConversions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OysterFx.AppCore.Domain.ValueObjects;

public class DescriptionConversion : ValueConverter<Description, string>
{
    public DescriptionConversion() : base(c => c.Value, c => c) { }
}