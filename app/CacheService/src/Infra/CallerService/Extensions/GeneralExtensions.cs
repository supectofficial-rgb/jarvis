namespace Insurance.CacheService.Infra.CallerService.Extensions;

using System.Collections;

public static class GeneralExtensions
{
    public static string ToQueryString(this object obj, string separator = ",")
    {
        if (obj == null)
            throw new ArgumentNullException("object");

        var properties = obj.GetType().GetProperties()
            .Where(x => x.CanRead)
            .Where(x => x.GetValue(obj, null) != null)
            .ToDictionary(x => x.Name, x => x.GetValue(obj, null));

        var propertyNames = properties
            .Where(x => !(x.Value is string) && x.Value is IEnumerable)
            .Select(x => x.Key)
            .ToList();

        foreach (var key in propertyNames)
        {
            var valueType = properties[key].GetType();
            var valueElemType = valueType.IsGenericType
                                    ? valueType.GetGenericArguments()[0]
                                    : valueType.GetElementType();
            if (valueElemType.IsPrimitive || valueElemType == typeof(string))
            {
                var enumerable = properties[key] as IEnumerable;
                properties[key] = string.Join(separator, enumerable.Cast<object>());
            }
        }

        return string.Join("&", properties
            .Select(x => string.Concat(
                Uri.EscapeDataString(x.Key), "=",
                Uri.EscapeDataString(x.Value.ToString()!))));
    }
}