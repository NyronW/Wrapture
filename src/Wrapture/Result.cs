using System.Text.Json;

namespace Wrapture;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; protected set; }

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Cannot have success result with error message.");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Cannot have failure result without error message.");

        IsSuccess = isSuccess;
        Error = error;
    }

    // --------------------
    // Factory methods
    // --------------------
    public static Result Success() => new Result(true, string.Empty);
    public static Result Failure(string error) => new Result(false, error);

    public static Result<T> Success<T>(T value)
        => new Result<T>(true, value, string.Empty);

    public static Result<T> Failure<T>(string error)
        => new Result<T>(false, default, error);

    public static Result SuccessIf(bool condition, string errorIfFalse)
        => condition ? Success() : Failure(errorIfFalse);

    public static Result FailureIf(bool condition, string errorIfTrue)
        => condition ? Failure(errorIfTrue) : Success();

    public static Result<T> SuccessIf<T>(bool condition, T value, string errorIfFalse)
        => condition ? Success(value) : Failure<T>(errorIfFalse);

    public static Result<T> FailureIf<T>(bool condition, T value, string errorIfTrue)
        => condition ? Failure<T>(errorIfTrue) : Success(value);

    // --------------------
    // Of (sync)
    // --------------------
    public static Result<T> Of<T>(Func<T> func)
    {
        try
        {
            var value = func();
            return Success(value);
        }
        catch (Exception ex)
        {
            return Failure<T>(ex.Message);
        }
    }

    public static Result Of(Action action)
    {
        try
        {
            action();
            return Success();
        }
        catch (Exception ex)
        {
            return Failure(ex.Message);
        }
    }

    // Of (async)
    public static async Task<Result<T>> Of<T>(Func<Task<T>> func)
    {
        try
        {
            var value = await func();
            return Success(value);
        }
        catch (Exception ex)
        {
            return Failure<T>(ex.Message);
        }
    }

    public static async Task<Result> Of(Func<Task> action)
    {
        try
        {
            await action();
            return Success();
        }
        catch (Exception ex)
        {
            return Failure(ex.Message);
        }
    }

    // --------------------
    // Map Methods
    // --------------------
    /// <summary>
    /// Transforms a successful Result into a new Result<U>.
    /// If this result is Failure, returns Failure(U) with the same error.
    /// </summary>
    public Result<U> Map<U>(Func<U> mapFunc)
    {
        if (IsFailure)
            return Failure<U>(Error);

        try
        {
            U value = mapFunc();
            return Success(value);
        }
        catch (Exception ex)
        {
            return Failure<U>(ex.Message);
        }
    }

    // --------------------
    // Implicit Conversion
    // --------------------
    public static implicit operator Result(string error) => new(false, error);

    public IReadOnlyCollection<string> GetErrors(char splitOn = ';') => Error?.Split(splitOn);

    public override string ToString()
    {
        return IsSuccess ? "Success" : $"Failure({Error})";
    }
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access Value of a failure result.");
            return _value!;
        }
    }

    public Result(bool isSuccess, T? value, string error) : base(isSuccess, error)
    {
        _value = value;
    }


    // --------------------
    // Map Methods
    // --------------------
    /// <summary>
    /// Map (sync): transforms the underlying Value (T -> U) if Success,
    /// otherwise propagates the same error as a Result<U>.
    /// </summary>
    public Result<U> Map<U>(Func<T, U> mapFunc)
    {
        if (IsFailure)
            return Failure<U>(Error);

        try
        {
            U newValue = mapFunc(Value);
            return Success(newValue);
        }
        catch (Exception ex)
        {
            return Failure<U>(ex.Message);
        }
    }

    /// <summary>
    /// Map (async): transforms the underlying Value (T -> Task<U>) if Success,
    /// otherwise propagates the same error as a Result<U>.
    /// </summary>
    public async Task<Result<U>> MapAsync<U>(Func<T, Task<U>> mapFunc)
    {
        if (IsFailure)
            return Failure<U>(Error);

        try
        {
            U newValue = await mapFunc(Value);
            return Success(newValue);
        }
        catch (Exception ex)
        {
            return Failure<U>(ex.Message);
        }
    }

    // --------------------
    // Implicit Conversion
    // --------------------
    public static implicit operator T(Result<T> result) => result.Value; // throws if failure
    public static implicit operator Result<T>(T value) => new(true, value, string.Empty);

    public override string ToString()
    {
        return IsSuccess ? $"Success({JsonSerializer.Serialize(Value)})" : $"Failure({Error})";
    }
}


public static class ResultExtensions
{
    public static Result Then(this Result result, Action action)
    {
        if (result.IsFailure)
        {
            return result;
        }

        try
        {
            action();
            return result;
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while executing the action: {ex.Message}");
        }
    }

    public static Result<T> Then<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsFailure)
        {
            return result;
        }

        try
        {
            action(result.Value);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Failure<T>($"An error occurred while executing the action: {ex.Message}");
        }
    }

    public static Result<T> ToErrorResult<T>(this Exception ex)
    {
        return Result.Failure<T>(ex.Message);
    }
}