using OysterFx.AppCore.Domain.Exceptions;

namespace OysterFx.AppCore.Domain.ValueObjects;

public class BusinessKey : ValueObject
{
    public static BusinessKey FromGuid(Guid value) => new() { Value = value };

    public Guid Value { get; private set; }

    private BusinessKey() { }
    protected BusinessKey(Guid value)
    {
        if (value == Guid.Empty)
            throw new InvalidValueObjectStateExceptions("ValidationErrorIsRequire", nameof(BusinessKey));

        Value = value;
    }
    public override string ToString()
    {
        return Value.ToString();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator string(BusinessKey title) => title.Value.ToString();
    public static implicit operator BusinessKey(string value) => new Guid(value);


    public static explicit operator Guid(BusinessKey title) => title.Value;
    public static implicit operator BusinessKey(Guid value) => new() { Value = value };
}