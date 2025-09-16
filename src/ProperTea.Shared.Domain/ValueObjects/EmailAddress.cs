using ProperTea.Shared.Domain.Exceptions;

namespace ProperTea.Shared.Domain.ValueObjects;

public record EmailAddress : ValueObject
{
    public const int MinLength = 1;
    public const int MaxLength = 200;

    private EmailAddress(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("EmailAddress.Required");

        return value.Length switch
        {
            > MaxLength => throw new DomainException("EmailAddress.NameTooLong"),
            < MinLength => throw new DomainException("EmailAddress.NameTooShort"),
            _ => new EmailAddress(value)
        };
    }

    public static implicit operator string(EmailAddress name)
    {
        return name.Value;
    }

    public static explicit operator EmailAddress(string value)
    {
        return Create(value);
    }
}