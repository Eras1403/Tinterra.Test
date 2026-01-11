namespace Tinterra.Application.Models;

public class Result<T>
{
    public bool Succeeded { get; }
    public string[] Errors { get; }
    public T? Value { get; }

    private Result(bool succeeded, T? value, string[] errors)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());
    public static Result<T> Failure(params string[] errors) => new(false, default, errors);
}
