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
}
