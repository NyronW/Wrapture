using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Wrapture;

/// <summary>
/// A lightweight optional type that may or may not contain a value of type T.
/// Implemented as a struct to avoid extra heap allocations for 'None'.
/// </summary>
/// <typeparam name="T">The underlying type of the optional value.</typeparam>
[Serializable]
public readonly struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<object>, IMaybe<T>
{
    private readonly bool _isValueSet;

    private readonly T? _value;

    /// <summary>
    /// True if this instance contains a value; otherwise false.
    /// </summary>
    public bool HasValue => _isValueSet;

    public bool HasNoValue => !HasValue;


    /// <summary>
    /// The value if present; throws if none.
    /// </summary>
    public T Value => GetValueOrThrow();

    // Private constructor for the Some case.
    private Maybe(T value)
    {
        if (value == null)
        {
            _isValueSet = false;
            _value = default;
            return;
        }

        _isValueSet = true;
        _value = value;
    }

    /// <summary>
    /// Represents a 'None' instance with no value.
    /// </summary>
    public static Maybe<T> None => new Maybe<T>();

    /// <summary>
    /// Creates a Maybe from a value. If the value is null, returns None.
    /// </summary>
    public static Maybe<T> From(T value)
    {
        return value is null ? None : new Maybe<T>(value);
    }

    // Implicit conversion from T to Maybe<T>
    public static implicit operator Maybe<T>(T value)
    {
        if (value is Maybe<T> m)
        {
            return m;
        }

        return Maybe.From(value);
    }

    public static implicit operator Maybe<T>(Maybe value) => None;

    // Optionally define an implicit from Maybe<T> to T if desired (risky if None).
    public static implicit operator T(Maybe<T> maybe) => maybe.Value;

    /// <summary>
    /// Returns the contained value if present, otherwise default(T).
    /// </summary>
    public T GetValueOrDefault() => HasValue ? _value! : default!;

    /// <summary>
    /// Returns the contained value if present, otherwise the fallback provided.
    /// </summary>
    public T GetValueOrDefault(T fallback) => HasValue ? _value! : fallback;

    /// <summary>
    /// Returns 'this' if it has a value, otherwise returns 'maybeOr'.
    /// </summary>
    public Maybe<T> Or(Maybe<T> maybeOr) => HasValue ? this : maybeOr;

    /// <summary>
    /// Converts this Maybe to a Success/Failure Result, using 'errorIfNone' if no value is present.
    /// </summary>
    public Result<T> ToResult(string errorIfNone)
    {
        return HasValue
            ? Result.Success(_value!)
            : Result.Failure<T>(errorIfNone);
    }

    /// <summary>
    ///  Indicates whether the inner value is present and returns the value if it is.
    /// </summary>
    /// <param name="value">The inner value, if present; otherwise `default`</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(
        [NotNullWhen(true), MaybeNullWhen(false)]
            out T value)
    {
        value = _value;
        return _isValueSet;
    }

    /// <summary>
    /// Returns the inner value if there's one, otherwise throws an InvalidOperationException with <paramref name="errorMessage"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">Maybe has no value.</exception>
    public T GetValueOrThrow(string? errorMessage = null)
    {
        if (HasNoValue)
            throw new InvalidOperationException(errorMessage ?? "Maybe has no value.");

        return _value!;
    }


    /// <summary>
    /// Throws the provided exception if no value is present.
    /// </summary>
    public T GetValueOrThrow(Exception exceptionIfNone)
    {
        if (!HasValue)
            throw exceptionIfNone;
        return _value!;
    }

    /// <summary>
    /// If this Maybe has a value, calls the specified action with the value.
    /// Returns 'this' (fluent style).
    /// </summary>
    public Maybe<T> Execute(Action<T> action)
    {
        if (HasValue)
        {
            action(_value!);
        }
        return this;
    }

    /// <summary>
    /// If this Maybe has no value, calls the specified action.
    /// Returns 'this' (fluent style).
    /// </summary>
    public Maybe<T> ExecuteNoValue(Action actionIfNone)
    {
        if (!HasValue)
        {
            actionIfNone();
        }
        return this;
    }

    // --------------------
    // Equality methods
    // --------------------
    public static bool operator ==(Maybe<T> maybe, T value)
    {
        if (value is Maybe<T>)
            return maybe.Equals(value);

        if (maybe.HasNoValue)
            return value is null;

        return maybe._value.Equals(value);
    }

    public static bool operator !=(Maybe<T> maybe, T value)
    {
        return !(maybe == value);
    }

    public static bool operator ==(Maybe<T> maybe, object other)
    {
        return maybe.Equals(other);
    }

    public static bool operator !=(Maybe<T> maybe, object other)
    {
        return !(maybe == other);
    }

    public static bool operator ==(Maybe<T> first, Maybe<T> second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(Maybe<T> first, Maybe<T> second)
    {
        return !(first == second);
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;
        if (obj is Maybe<T> other)
            return Equals(other);
        if (obj is T value)
            return Equals(value);
        return false;
    }

    public bool Equals(Maybe<T> other)
    {
        if (HasNoValue && other.HasNoValue)
            return true;

        if (HasNoValue || other.HasNoValue)
            return false;

        return EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    public override int GetHashCode()
    {
        if (HasNoValue)
            return 0;

        return _value.GetHashCode();
    }

    public override string ToString() => HasValue ? _value!.ToString()! : "No value";
}

/// <summary>
/// Non-generic entrypoint for <see cref="Maybe{T}" /> members
/// </summary>
public readonly struct Maybe
{
    public static Maybe None => new Maybe();

    /// <summary>
    /// Creates a new <see cref="Maybe{T}" /> from the provided <paramref name="value"/>
    /// </summary>
    public static Maybe<T> From<T>(T value) => Maybe<T>.From(value);
}

/// <summary>
/// Useful in scenarios where you need to determine if a value is Maybe or not
/// </summary>
public interface IMaybe<out T>
{
    T Value { get; }
    bool HasValue { get; }
    bool HasNoValue { get; }
}

public static class MaybeExtensions
{
    public static async Task<TResult> MatchAsync<T, TResult>(this Maybe<T> maybe,
                                                         Func<T, Task<TResult>> someAsync,
                                                         Func<Task<TResult>> noneAsync)
    {
        if (maybe.HasValue)
        {
            return await someAsync(maybe.Value);
        }

        return await noneAsync();
    }

    public static async Task<Maybe<TTarget>> MapAsync<TSource, TTarget>(this Maybe<TSource> maybe,
                                                                        Func<TSource, Task<TTarget>> transform)
    {
        return await maybe.MatchAsync(
            async value => Maybe<TTarget>.From(await transform(value)),
            () => Task.FromResult(Maybe<TTarget>.None));
    }

    public static Maybe<TTarget> Map<TSource, TTarget>(this Maybe<TSource> maybe,
                                                        Func<TSource, TTarget> transform)
    {
        if (maybe.HasValue)
            return Maybe.From(transform(maybe.Value));

        return Maybe<TTarget>.None;
    }
}