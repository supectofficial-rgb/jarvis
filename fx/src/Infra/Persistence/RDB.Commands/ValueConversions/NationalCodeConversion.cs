namespace OysterFx.Infra.Persistence.RDB.Commands.ValueConversions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OysterFx.AppCore.Domain.ValueObjects;

public class NationalCodeConversion : ValueConverter<NationalCode, string>
{
    public NationalCodeConversion() : base(c => c.Value, c => c) { }
}