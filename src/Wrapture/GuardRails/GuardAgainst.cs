namespace Wrapture.GuardRails;

public abstract class GuardAgainstBase
{
    private readonly List<string> _errors = [];
    private readonly SanityCheckMode _checkMode;

    protected GuardAgainstBase(SanityCheckMode checkMode = SanityCheckMode.Full)
    {
        _checkMode = checkMode;
    }
    internal void AddError(string errorMessage)
    {
        _errors.Add(errorMessage);
    }

    internal bool HasErrors => _errors.Any();

    internal string GetErrors() => string.Join("; ", _errors);

    public bool ShouldSkipValidation() => _checkMode == SanityCheckMode.ShortCircuit && HasErrors;
}

public enum SanityCheckMode
{
    ShortCircuit,
    Full
}

public class GuardAgainst : GuardAgainstBase
{
    public GuardAgainst(SanityCheckMode checkMode = SanityCheckMode.Full)
         : base(checkMode)
    {
    }


    public Result ToResult()
    {
        if (HasErrors)
        {
            return Result.Failure(GetErrors());
        }
        return Result.Success();
    }

    public Result<T> ToResult<T>(Func<T> constructor)
    {
        if (HasErrors)
        {
            return Result.Failure<T>(GetErrors());
        }

        try
        {
            var result = constructor();
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<T>($"An error occurred while creating the object: {ex.Message}");
        }
    }

    public Result Then(Action action)
    {
        if (HasErrors)
        {
            return Result.Failure(GetErrors());
        }

        try
        {
            action(); // Execute the action
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while executing the action: {ex.Message}");
        }
    }

    public Result<T> Then<T>(Func<T> action)
    {
        if (HasErrors)
        {
            return Result.Failure<T>(GetErrors());
        }

        try
        {
            var result = action(); // Execute the action and return its result
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<T>($"An error occurred while executing the action: {ex.Message}");
        }
    }
}

public static class Guard
{
    public static GuardAgainst Against() => new();
}


public static class GuardAgainstExtensions
{
    public static T Null<T, U>(this T sanityCheck, U value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value is null)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static T Predicate<T>(this T sanityCheck, Func<bool> predicate, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (predicate())
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }
}