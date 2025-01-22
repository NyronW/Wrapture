namespace Wrapture.GuardRails;

public static class GuardAgainstNumericExtensions
{
    public static T LessThanOrEqualZero<T>(this T sanityCheck, int value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value <= 0)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static T LessThan<T>(this T sanityCheck, int value, int threshold, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value < threshold)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }
}
