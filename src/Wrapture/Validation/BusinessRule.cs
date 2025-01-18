namespace Wrapture.Validation;

public abstract class BusinessRule<T>
{
    public abstract Task<Result> EvaluateAsync(T context);
}
