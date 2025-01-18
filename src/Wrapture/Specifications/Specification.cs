using System.Linq.Expressions;

namespace Wrapture.Specifications;

public abstract class Specification<T>
{
    private Func<T, bool>? _compiledExpression;

    public static readonly Specification<T> All = new NoSpecification<T>();

    public bool IsSatisfiedBy(T entity)
    {
        // Use the cached compiled expression if available
        var predicate = _compiledExpression ??= ToExpression().Compile();
        return predicate(entity);
    }

    public abstract Expression<Func<T, bool>> ToExpression();

    public Specification<T> And(Specification<T> other)
    {
        if (this == All) return other;
        if (other == All) return this;

        return new AndSpecification<T>(this, other);
    }

    public Specification<T> And(Expression<Func<T, bool>> predicate)
    {
        return And(new ExpressionSpecification<T>(predicate));
    }

    public Specification<T> Or(Specification<T> other)
    {
        if (this == All || other == All) return All;

        return new OrSpecification<T>(this, other);
    }

    public Specification<T> Or(Expression<Func<T, bool>> predicate)
    {
        return Or(new ExpressionSpecification<T>(predicate));
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }

    // Reset the cache if needed (e.g., if specifications are dynamically altered)
    protected void ResetCache() => _compiledExpression = null;
}

internal sealed class NoSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return entity => true; // Always true, no specific criteria
    }
}

internal sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        // Use the parameter from the left expression for both expressions
        var parameter = leftExpression.Parameters[0];

        // Adjust the right expression to use the same parameter
        var body = Expression.AndAlso(
            leftExpression.Body,
            new ParameterReplacer(rightExpression.Parameters[0], parameter).Visit(rightExpression.Body)
        );

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    // Helper class to replace parameters in an expression
    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter ?? throw new ArgumentNullException(nameof(oldParameter));
            _newParameter = newParameter ?? throw new ArgumentNullException(nameof(newParameter));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            // Replace the old parameter with the new one
            return node == _oldParameter ? _newParameter : node;
        }
    }
}

internal sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.OrElse(leftExpression.Body, rightExpression.Body);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}

internal class ExpressionSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    public ExpressionSpecification(Expression<Func<T, bool>> expression)
    {
        _expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        return _expression;
    }
}

internal sealed class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _specification;

    public NotSpecification(Specification<T> specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var originalExpression = _specification.ToExpression();
        var notExpression = Expression.Not(originalExpression.Body);

        return Expression.Lambda<Func<T, bool>>(notExpression, originalExpression.Parameters);
    }
}