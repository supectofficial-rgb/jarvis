namespace Insurance.Infra.Persistence.Sql.Commands.Losts.ValueConversions;

using Insurance.AppCore.Domain.BaseData.VehiclePlates.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class VehiclePlateConversion : ValueConverter<VehiclePlate, string>
{
    public VehiclePlateConversion()
        : base(
            v => v.ToString(),
            v => ParseFromString(v))
    { }

    private static VehiclePlate ParseFromString(string value)
    {
        // مثال: "12 ب 345 18"
        var parts = value.Split(' ');
        if (parts.Length >= 3)
        {
            var vehiclePlate = VehiclePlate.CreateTaxiPlate(parts[0], parts[1], parts[2], parts.Length > 3 ? parts[3] : null);
            return vehiclePlate;
        }
        throw new InvalidOperationException("فرمت رشته پلاک نامعتبر است");
    }
}