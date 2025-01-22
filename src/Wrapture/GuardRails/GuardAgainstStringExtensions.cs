namespace Wrapture.GuardRails;

public static class GuardAgainstStringExtensions
{
    public static T NullOrEmpty<T>(this T sanityCheck, string value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (string.IsNullOrEmpty(value))
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static T NullOrWhiteSpace<T>(this T sanityCheck, string value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (string.IsNullOrWhiteSpace(value))
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static T MaxLength<T>(this T sanityCheck, string value, int maxLength, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }
}
