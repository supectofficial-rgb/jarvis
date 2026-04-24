using OysterFx.AppCore.Domain.Exceptions;
using OysterFx.AppCore.Domain.Extensions;

namespace OysterFx.AppCore.Domain.ValueObjects;
public class LegalNationalId : ValueObject
{
    #region Properties
    public string Value { get; private set; }
    #endregion

    #region Constructors and Factories
    public static LegalNationalId FromString(string value) => new(value);
    private LegalNationalId(string value)
    {
        if (!value.IsLegalNationalIdValid())
        {
            throw new InvalidValueObjectStateExceptions("ValidationErrorStringFormat", nameof(LegalNationalId));
        }

        Value = value;
    }
    private LegalNationalId() { }

    #endregion

    #region Equality Check

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    #endregion

    #region Operator Overloading
    public static explicit operator string(LegalNationalId title) => title.Value;
    public static implicit operator LegalNationalId(string value) => new(value);
    #endregion

    #region Methods
    public override string ToString() => Value;

    #endregion
}