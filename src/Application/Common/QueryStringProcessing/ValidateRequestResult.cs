namespace Application.Common.QueryStringProcessing;

public record ValidationRequestResult<TResult, TError>(TResult? Result = null, TError? Error = null)
    where TResult : class
    where TError : class;
