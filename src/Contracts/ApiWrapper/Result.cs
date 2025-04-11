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
}

public static class ResultExtension
{
    public static TReturn Match<TResult, TReturn>(
        this Result<TResult> result,
        Func<TResult, TReturn> onSuccess,
        Func<ErrorDetails, TReturn> onFailure
    )
        where TResult : class
    {
        return result.IsSuccess ? onSuccess(result.Value!) : onFailure(result.Error!);
    }
}
