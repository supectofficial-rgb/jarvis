namespace OysterFx.Infra.Persistence.RDB.Commands.ValueConversions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OysterFx.AppCore.Domain.ValueObjects;

public class TitleConversion : ValueConverter<Title, string>
{
    public TitleConversion() : base(c => c.Value, c => c) { }
}