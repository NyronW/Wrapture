using FluentAssertions;

namespace Wrapture.Tests.Maybe;

public class MaybeTests
{
    [Fact]
    public void Maybe_Should_Create_Some()
    {
        // Act
        var maybe = Wrapture.Maybe.From(42);

        // Assert
        maybe.HasValue.Should().BeTrue();
        maybe.Value.Should().Be(42);
    }

    [Fact]
    public void Maybe_Should_Create_None()
    {
        // Act
        var maybe = Maybe<int>.None;

        // Assert
        maybe.HasValue.Should().BeFalse();
        Action accessValue = () => { var _ = maybe.Value; };
        accessValue.Should().Throw<InvalidOperationException>()
            .WithMessage("Maybe has no value.");
    }

    [Fact]
    public void Maybe_Map_Should_Transform_Value_When_Some()
    {
        // Arrange
        var maybe = Wrapture.Maybe.From(10);

        // Act
        var mappedMaybe = maybe.Map(x => x * 2);

        // Assert
        mappedMaybe.HasValue.Should().BeTrue();
        mappedMaybe.Value.Should().Be(20);
    }

    [Fact]
    public void Maybe_Map_Should_Return_None_When_None()
    {
        // Arrange
        var maybe = Maybe<int>.None;

        // Act
        var mappedMaybe = maybe.Map(x => x * 2);

        // Assert
        mappedMaybe.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void Maybe_GetValueOrDefault_Should_Return_Value_When_Some()
    {
        // Arrange
        var maybe = Wrapture.Maybe.From(42);

        // Act
        var result = maybe.GetValueOrDefault();
        var resultWithFallback = maybe.GetValueOrDefault(100);

        // Assert
        result.Should().Be(42);
        resultWithFallback.Should().Be(42);
    }

    [Fact]
    public void Maybe_GetValueOrDefault_Should_Return_Default_When_None()
    {
        // Arrange
        var maybe = Maybe<int>.None;

        // Act
        var result = maybe.GetValueOrDefault();
        var resultWithFallback = maybe.GetValueOrDefault(100);

        // Assert
        result.Should().Be(0); // default(int)
        resultWithFallback.Should().Be(100);
    }

    [Fact]
    public void Maybe_Or_Should_Return_Alternative_When_None()
    {
        // Arrange
        var maybe = Maybe<int>.None;
        var alternative = Wrapture.Maybe.From(42);

        // Act
        var result = maybe.Or(alternative);

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Maybe_ToResult_Should_Convert_To_Result()
    {
        // Arrange
        var someValue = Wrapture.Maybe.From(42);
        var noneValue = Maybe<int>.None;

        // Act
        var successResult = someValue.ToResult("Error");
        var failureResult = noneValue.ToResult("Value was none");

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        successResult.Value.Should().Be(42);

        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().Be("Value was none");
    }

    [Fact]
    public void Maybe_TryGetValue_Should_Return_Correct_Result()
    {
        // Arrange
        var someValue = Wrapture.Maybe.From(42);
        var noneValue = Maybe<int>.None;

        // Act & Assert
        someValue.TryGetValue(out var value).Should().BeTrue();
        value.Should().Be(42);

        noneValue.TryGetValue(out var noneResult).Should().BeFalse();
        noneResult.Should().Be(0); // default(int)
    }

    [Fact]
    public void Maybe_GetValueOrThrow_Should_Handle_Both_Cases()
    {
        // Arrange
        var someValue = Wrapture.Maybe.From(42);
        var noneValue = Maybe<int>.None;

        // Act & Assert
        someValue.GetValueOrThrow().Should().Be(42);
        someValue.GetValueOrThrow("Custom error").Should().Be(42);
        someValue.GetValueOrThrow(new ArgumentException("Custom exception")).Should().Be(42);

        Action getNoValue = () => noneValue.GetValueOrThrow();
        getNoValue.Should().Throw<InvalidOperationException>()
            .WithMessage("Maybe has no value.");

        Action getNoValueCustomMessage = () => noneValue.GetValueOrThrow("Custom error");
        getNoValueCustomMessage.Should().Throw<InvalidOperationException>()
            .WithMessage("Custom error");

        var customException = new ArgumentException("Custom exception");
        Action getNoValueCustomException = () => noneValue.GetValueOrThrow(customException);
        getNoValueCustomException.Should().Throw<ArgumentException>()
            .WithMessage("Custom exception");
    }

    [Fact]
    public void Maybe_Execute_Should_Run_Actions_Correctly()
    {
        // Arrange
        var someValue = Wrapture.Maybe.From(42);
        var noneValue = Maybe<int>.None;
        var sideEffectValue = 0;

        // Act
        someValue.Execute(x => sideEffectValue = x);
        var someResult = sideEffectValue;

        noneValue.Execute(x => sideEffectValue = x);
        var noneResult = sideEffectValue;

        // Assert
        someResult.Should().Be(42);
        noneResult.Should().Be(42); // Unchanged from previous value
    }

    [Fact]
    public void Maybe_ExecuteNoValue_Should_Run_Actions_Correctly()
    {
        // Arrange
        var someValue = Wrapture.Maybe.From(42);
        var noneValue = Maybe<int>.None;
        var wasExecuted = false;

        // Act
        someValue.ExecuteNoValue(() => wasExecuted = true);
        var someResult = wasExecuted;

        wasExecuted = false;
        noneValue.ExecuteNoValue(() => wasExecuted = true);
        var noneResult = wasExecuted;

        // Assert
        someResult.Should().BeFalse();
        noneResult.Should().BeTrue();
    }

    [Fact]
    public async Task Maybe_MapAsync_Should_Transform_Value_Asynchronously()
    {
        // Arrange
        var maybe = Wrapture.Maybe.From(10);

        // Act
        var mappedMaybe = await maybe.MapAsync(async x => 
        {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert
        mappedMaybe.HasValue.Should().BeTrue();
        mappedMaybe.Value.Should().Be(20);
    }

    [Fact]
    public async Task Maybe_MatchAsync_Should_Execute_Correct_Function()
    {
        // Arrange
        var someValue = Wrapture.Maybe.From(42);
        var noneValue = Maybe<int>.None;

        // Act
        var someResult = await someValue.MatchAsync<int, string>(
            async x => 
            {
                await Task.Delay(1);
                return $"Some: {x}";
            },
            async () => 
            {
                await Task.Delay(1);
                return "None";
            }
        );

        var noneResult = await noneValue.MatchAsync<int, string>(
            async x => 
            {
                await Task.Delay(1);
                return $"Some: {x}";
            },
            async () => 
            {
                await Task.Delay(1);
                return "None";
            }
        );

        // Assert
        someResult.Should().Be("Some: 42");
        noneResult.Should().Be("None");
    }
}
