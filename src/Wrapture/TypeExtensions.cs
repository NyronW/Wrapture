namespace System;

public static class TypeExtensions
{
    public static bool IsEmpty(this Guid type)  => type == Guid.Empty;
    public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
    public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

    public static bool Equals(this string value, string other) => string.Equals(value, other);
    public static bool NotEquals(this string value, string other) => !string.Equals(value, other);
    public static bool Equals(this string value, string other, StringComparison comparisonType) => string.Equals(value, other, comparisonType);
    public static bool NotEquals(this string value, string other, StringComparison comparisonType) => string.Equals(value, other, comparisonType);
    public static bool LessThan<T>(this T value, T threshold) where T : IComparable => value.CompareTo(threshold) < 0;
    public static bool GreaterThan<T>(this T value, T threshold) where T : IComparable => value.CompareTo(threshold) > 0;
    public static T Coalesce<T>(this T? value, T other) where T : struct => value ?? other;
    public static T Coalesce<T>(this T value, T other) where T : class => value ?? other;


    internal static T LessThan<T>(this T value, T threshold, string message) where T : IComparable
    {
        if (value.CompareTo(threshold) < 0)
            throw new ArgumentException(message);
        return value;
    }
    internal static T GreaterThan<T>(this T value, T threshold, string message) where T : IComparable
    {
        if (value.CompareTo(threshold) > 0)
            throw new ArgumentException(message);
        return value;
    }

}
