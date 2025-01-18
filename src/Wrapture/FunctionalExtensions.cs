namespace Wrapture;

public static class FunctionalExtensions
{
    /// <summary>
    /// Executes a side effect without modifying the input.
    /// </summary>
    /// <remarks>result.Tap(r => Console.WriteLine($"Operation result: {r}"));</remarks>
    /// <returns>Instance of T</returns>
    public static T Tap<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }

    /// <summary>
    /// Facilitates method chaining and functional pipelines.
    /// </summary>
    /// <remarks>var result = input.Pipe(x => x * 2).Pipe(x => x.ToString());</remarks>
    /// <returns></returns>
    public static TResult Pipe<TInput, TResult>(this TInput input, Func<TInput, TResult> func)
    {
        return func(input);
    }

}
