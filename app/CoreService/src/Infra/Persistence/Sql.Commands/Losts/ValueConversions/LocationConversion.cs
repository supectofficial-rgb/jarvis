namespace Insurance.Infra.Persistence.Sql.Commands.Losts.ValueConversions;

using Insurance.AppCore.Domain.Losts.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;
using System.Text.Json;

public class LocationConversion : ValueConverter<Location, string>
{
    public LocationConversion()
        : base(
            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
            v => JsonSerializer.Deserialize<Location>(v, JsonSerializerOptions.Default)
                 ?? CreateEmptyLocation())
    { }

    private static Location CreateEmptyLocation()
    {
        var constructor = typeof(Location).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(decimal), typeof(decimal), typeof(string), typeof(string) },
            null);

        return (Location)constructor.Invoke(new object[] { 0, 0, null, null });
    }
}