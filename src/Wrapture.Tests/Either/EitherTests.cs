using Xunit;
using FluentAssertions;

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
}
