namespace Wrapture;

public abstract class Either<L, R>
{
    public abstract T Match<T>(Func<L, T> onLeft, Func<R, T> onRight);

    public static Either<L, R> Left(L value) => new LeftValue<L, R>(value);
    public static Either<L, R> Right(R value) => new RightValue<L, R>(value);

    public bool IsLeft => this is LeftValue<L, R>;
    public bool IsRight => this is RightValue<L, R>;

    private sealed class LeftValue<L, R> : Either<L, R>
    {
        private readonly L _value;

        public LeftValue(L value) => _value = value;

        public override T Match<T>(Func<L, T> onLeft, Func<R, T> onRight) => onLeft(_value);
    }

    private sealed class RightValue<L, R> : Either<L, R>
    {
        private readonly R _value;

        public RightValue(R value) => _value = value;

        public override T Match<T>(Func<L, T> onLeft, Func<R, T> onRight) => onRight(_value);
    }

    public Either<L, T> Map<T>(Func<R, T> mapFunc)
    {
        return Match(
            onLeft: l => Either<L, T>.Left(l),
            onRight: r => Either<L, T>.Right(mapFunc(r))
        );
    }

    public Either<L, T> Bind<T>(Func<R, Either<L, T>> bindFunc)
    {
        return Match(
            onLeft: l => Either<L, T>.Left(l),
            onRight: bindFunc
        );
    }

    public async Task<Either<L, T>> MapAsync<T>(Func<R, Task<T>> mapFunc)
    {
        return await Match(
            onLeft: l => Task.FromResult(Either<L, T>.Left(l)),
            onRight: async r => Either<L, T>.Right(await mapFunc(r))
        );
    }

    public async Task<Either<L, T>> BindAsync<T>(Func<R, Task<Either<L, T>>> bindFunc)
    {
        return await Match(
            onLeft: l => Task.FromResult(Either<L, T>.Left(l)),
            onRight: bindFunc
        );
    }
}
