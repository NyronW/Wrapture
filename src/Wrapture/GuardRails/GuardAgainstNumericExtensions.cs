using System.Numerics;

namespace Wrapture.GuardRails;

public static class GuardAgainstNumericExtensions
{
    public static TGuard LessThanOrEqualZero<TGuard, TNumeric>(
           this TGuard sanityCheck,
           TNumeric value,
           string errorMessage)
           where TGuard : GuardAgainstBase
           where TNumeric : INumber<TNumeric>
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value <= TNumeric.Zero)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static TGuard LessThan<TGuard, TNumeric>(
        this TGuard sanityCheck,
        TNumeric value,
        TNumeric threshold,
        string errorMessage)
        where TGuard : GuardAgainstBase
        where TNumeric : INumber<TNumeric>
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value < threshold)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static TGuard GreaterThanZero<TGuard, TNumeric>(
        this TGuard sanityCheck,
        TNumeric value,
        string errorMessage)
        where TGuard : GuardAgainstBase
        where TNumeric : INumber<TNumeric>
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value <= TNumeric.Zero)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }

    public static TGuard GreaterThan<TGuard, TNumeric>(
        this TGuard sanityCheck,
        TNumeric value,
        TNumeric threshold,
        string errorMessage)
        where TGuard : GuardAgainstBase
        where TNumeric : INumber<TNumeric>
    {
        if (sanityCheck.ShouldSkipValidation())
            return sanityCheck;

        if (value <= threshold)
        {
            sanityCheck.AddError(errorMessage);
        }
        return sanityCheck;
    }
}
