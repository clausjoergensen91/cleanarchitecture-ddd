using Domain.Primitives;
using Domain.Shared;

public sealed class Email : ValueObject {
    public const int MaxLength = 50;

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<Email>(new Error(
                "Email.Empty",
                "Email is empty."));
        }

        if (email.Length > MaxLength)
        {
            return Result.Failure<Email>(new Error(
                "Email.TooLong",
                "Email is too long."));
        }

        return new Email(email);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
