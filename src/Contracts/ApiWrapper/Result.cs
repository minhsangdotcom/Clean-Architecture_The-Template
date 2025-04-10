namespace Contracts.ApiWrapper;

public sealed class Result<TResult>
    where TResult : class
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TResult? Value { get; }

    public ErrorDetails? Error { get; set; }

    private Result(bool isSuccess, TResult? value, ErrorDetails? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<TResult> Success()
    {
        return new Result<TResult>(true, null, null);
    }

    public static Result<TResult> Success(TResult value)
    {
        return new Result<TResult>(true, value, null);
    }

    public static Result<TResult> Failure<TError>(TError error)
        where TError : ErrorDetails
    {
        return new Result<TResult>(false, default, error);
    }

    public static TResult Match(
        Result<TResult> result,
        Func<TResult> onSuccess,
        Func<ErrorDetails, TResult> onFailure
    )
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
    }
}
