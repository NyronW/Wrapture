using FluentAssertions;
using Wrapture.GuardRails;

namespace Wrapture.Tests.Guards;

public class GuardAgainstTests
{
    [Fact]
    public void GuardAgainst_NullOrWhiteSpace_Should_Throw_ArgumentException_When_Input_Is_Null()
    {
        // Arrange
        string input = null;

        // Act
        var result = Guard.Against().NullOrWhiteSpace(input, "Input cannot be null or empty")
            .ToResult();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Input cannot be null or empty");
    }

    [Fact]
    public void GuardAgainst_NullOrWhiteSpace_Should_Not_Throw_When_Input_Is_Valid()
    {
        // Arrange
        string input = "Valid input";

        // Act
        Action act = () => Guard.Against().NullOrWhiteSpace(input, "Input cannot be null or empty");

        // Assert
        act.Should().NotThrow();
    }
}