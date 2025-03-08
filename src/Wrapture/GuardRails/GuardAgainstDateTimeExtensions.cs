namespace Wrapture.GuardRails;

public static class GuardAgainstDateTimeExtensions
{
    public static T FutureDates<T>(this T sanityCheck, DateTime value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value > DateTime.Now)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static T PastDates<T>(this T sanityCheck, DateTime value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value < DateTime.Now)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static T FutureDates<T>(this T sanityCheck, DateOnly value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value > DateOnly.FromDateTime(DateTime.Now))
        {
            sanityCheck.AddError(errorMessage);
        }

        return sanityCheck;
    }

    public static T PastDates<T>(this T sanityCheck, DateOnly value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value < DateOnly.FromDateTime(DateTime.Now))
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }
}