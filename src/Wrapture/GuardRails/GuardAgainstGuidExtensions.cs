namespace Wrapture.GuardRails;

public static class GuardAgainstGuidExtensions
{
    public static T Empty<T>(this T sanityCheck, Guid value, string errorMessage) where T : GuardAgainstBase
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value == Guid.Empty)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }
}
