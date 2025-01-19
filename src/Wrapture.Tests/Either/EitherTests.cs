using Xunit;
using FluentAssertions;
using System;
using System.Threading.Tasks;

namespace Wrapture.Tests.Either;

public class EitherTests
{
    [Fact]
    public void Either_Should_Create_Left()
    {
        // Act
        var either = Either<string, int>.Left("Error");

        // Assert
        either.IsLeft.Should().BeTrue();
        either.IsRight.Should().BeFalse();

        var result = either.Match(
            left => $"Left: {left}",
            right => $"Right: {right}"
        );

        result.Should().Be("Left: Error");
    }

    [Fact]
    public void Either_Should_Create_Right()
    {
        // Act
        var either = Either<string, int>.Right(42);

        // Assert
        either.IsLeft.Should().BeFalse();
        either.IsRight.Should().BeTrue();

        var result = either.Match(
            left => $"Left: {left}",
            right => $"Right: {right}"
        );

        result.Should().Be("Right: 42");
    }

    [Fact]
    public void Either_Map_Should_Transform_Right_Value()
    {
        // Arrange
        var either = Either<string, int>.Right(10);

        // Act
        var mappedEither = either.Map(x => x * 2);

        // Assert
        mappedEither.IsRight.Should().BeTrue();
        mappedEither.Match(
            left => throw new Exception("Unexpected Left"),
            right => right
        ).Should().Be(20);
    }

    [Fact]
    public void Either_Bind_Should_Chain_Operations()
    {
        // Arrange
        var either = Either<string, int>.Right(10);

        // Act
        var result = either.Bind(x => 
            x > 0 ? Either<string, string>.Right(x.ToString()) 
                  : Either<string, string>.Left("Number must be positive"));

        // Assert
        result.IsRight.Should().BeTrue();
        result.Match(
            left => throw new Exception("Unexpected Left"),
            right => right
        ).Should().Be("10");
    }

    [Fact]
    public void Either_Tap_Should_Execute_Side_Effects()
    {
        // Arrange
        var either = Either<string, int>.Right(42);
        var sideEffectValue = 0;

        // Act
        either.Tap(
            left => throw new Exception("Should not execute left side effect"),
            right => sideEffectValue = right
        );

        // Assert
        sideEffectValue.Should().Be(42);
    }

    [Fact]
    public void Either_ToResult_Should_Convert_To_Result()
    {
        // Arrange
        var successEither = Either<string, int>.Right(42);
        var failureEither = Either<string, int>.Left("Error message");

        // Act
        var successResult = successEither.ToResult(error => error);
        var failureResult = failureEither.ToResult(error => error);

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        successResult.Value.Should().Be(42);

        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().Be("Error message");
    }

    [Fact]
    public async Task Either_MapAsync_Should_Transform_Right_Value_Asynchronously()
    {
        // Arrange
        var either = Either<string, int>.Right(10);

        // Act
        var mappedEither = await either.MapAsync(async x => 
        {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert
        mappedEither.IsRight.Should().BeTrue();
        mappedEither.Match(
            left => throw new Exception("Unexpected Left"),
            right => right
        ).Should().Be(20);
    }

    [Fact]
    public async Task Either_BindAsync_Should_Chain_Async_Operations()
    {
        // Arrange
        var either = Either<string, int>.Right(10);

        // Act
        var result = await either.BindAsync(async x => 
        {
            await Task.Delay(1);
            return x > 0 
                ? Either<string, string>.Right(x.ToString())
                : Either<string, string>.Left("Number must be positive");
        });

        // Assert
        result.IsRight.Should().BeTrue();
        result.Match(
            left => throw new Exception("Unexpected Left"),
            right => right
        ).Should().Be("10");
    }

    [Fact]
    public async Task Either_MatchAsync_Should_Execute_Correct_Async_Function()
    {
        // Arrange
        var rightEither = Either<string, int>.Right(42);
        var leftEither = Either<string, int>.Left("Error");

        // Act
        var rightResult = await rightEither.MatchAsync(
            onLeftAsync: async _ => 
            {
                await Task.Delay(1);
                return "Left";
            },
            onRightAsync: async x => 
            {
                await Task.Delay(1);
                return $"Right: {x}";
            }
        );

        var leftResult = await leftEither.MatchAsync(
            onLeftAsync: async x => 
            {
                await Task.Delay(1);
                return $"Left: {x}";
            },
            onRightAsync: async _ => 
            {
                await Task.Delay(1);
                return "Right";
            }
        );

        // Assert
        rightResult.Should().Be("Right: 42");
        leftResult.Should().Be("Left: Error");
    }

    [Fact]
    public async Task Either_TapAsync_Should_Execute_Async_Side_Effects()
    {
        // Arrange
        var either = Either<string, int>.Right(42);
        var sideEffectValue = 0;

        // Act
        await either.TapAsync(
            onLeftAsync: async _ => await Task.Delay(1),
            onRightAsync: async x => 
            {
                await Task.Delay(1);
                sideEffectValue = x;
            }
        );

        // Assert
        sideEffectValue.Should().Be(42);
    }
}
