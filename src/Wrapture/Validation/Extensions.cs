namespace Wrapture.Validation;

public static class Extensions
{
    public static async Task<Result> ApplyRulesAsync<T>(
    this T context,
    params BusinessRule<T>[] rules)
    {
        foreach (var rule in rules)
        {
            var result = await rule.EvaluateAsync(context);
            if (!result.IsSuccess)
                return result;
        }

        return Result.Success();
    }
}

