using SolarX.SERVICE.Abstractions.Base;

namespace SolarX.SERVICE.Services.Base;

public class Result<T> : IResult<T>
{
    public string Message { get; set; }
    public T Data { get; set; }
    public int StatusCode { get; set; }

    public static Result<T> CreateResult(string message, int statusCode, T data)
    {
        return new Result<T>()
        {
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }
}

public class Result : IResult
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public object? Data { get; set; }

    public int SuccessCount { get; set; }

    public static Result CreateResult(string message, int statusCode, object? data = null)
    {
        return new Result()
        {
            Message = message,
            StatusCode = statusCode,
            Data = data
        };
    }
}