namespace OysterFx.Infra.Persistence.RDB.Commands.ValueConversions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OysterFx.AppCore.Domain.ValueObjects;

public class LegalNationalIdConversion : ValueConverter<LegalNationalId, string>
{
    public LegalNationalIdConversion() : base(c => c.Value, c => LegalNationalId.FromString(c)) { }
}