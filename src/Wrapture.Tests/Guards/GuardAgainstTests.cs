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
        Result<string> act = Guard.Against().NullOrWhiteSpace(input, "Input cannot be null or empty")
            .ToResult<string>(() => "hello world");

        // Assert
        act.IsSuccess.Should().Be(true);
        act.Value.Should().Be("hello world");
    }

    [Fact]
    public void GuardAgainst_LessThanOrEqualZero_Should_Not_Throw_When_Input_Is_Valid()
    {
        // Arrange
        int input = 10;

        // Act
        Result act = Guard.Against().LessThanOrEqualZero(input, "Input cannot be less than or equal to zero")
            .ToResult();

        // Assert
        act.IsSuccess.Should().Be(true);
    }

    [Fact]
    public void GuardAgainst_LessThan_Should_Not_Be_Success_When_Input_Is_Not_Valid()
    {
        // Arrange
        decimal input = 10;

        // Act
        Result act = Guard.Against().LessThan(input, 20, "Input cannot be less than 20")
            .ToResult();

        // Assert
        act.IsSuccess.Should().Be(false);
    }
}