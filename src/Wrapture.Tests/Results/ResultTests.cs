using Xunit;
using FluentAssertions;
using Wrapture; // Replace with your actual Wrapture namespace

namespace Wrapture.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Result_Should_Create_Success()
    {
        // Arrange & Act
        var result = Result.Success("Success");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Success");
    }

    [Fact]
    public void Result_Should_Create_Failure()
    {
        // Arrange & Act
        var result = Result.Failure<string>("Error");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Error");
    }

    [Fact]
    public void Result_Map_Should_Transform_Value_On_Success()
    {
        // Arrange
        var result = Result.Success(10);

        // Act
        var mappedResult = result.Map(x => x * 2);

        // Assert
        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be(20);
    }
}
