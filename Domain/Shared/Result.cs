namespace Domain.Shared;
public class Result {

    protected internal Result(bool isSuccess, Error error)
    {
        if(isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSucces;
        Error = error;
    }

    public bool IsSuccess { get; set; }
    public bool IsFailure => !isSuccess;
    public Error Error { get; }
    public static Result Success() => new(true, Error.None);
    public static Result<TValue> Success<TValue>(TValue value => new (value))
}
