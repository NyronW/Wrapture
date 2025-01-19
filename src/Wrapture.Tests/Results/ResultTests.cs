using Xunit;
using FluentAssertions;
using Wrapture; // Replace with your actual Wrapture namespace
using System;
using System.Threading.Tasks;

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

    [Fact]
    public void Result_Map_Should_Not_Transform_Value_On_Failure()
    {
        // Arrange
        var result = Result.Failure<int>("Error message");

        // Act
        var mappedResult = result.Map(x => x * 2);

        // Assert
        mappedResult.IsFailure.Should().BeTrue();
        mappedResult.Error.Should().Be("Error message");
    }

    [Fact]
    public async Task Result_MapAsync_Should_Transform_Value_On_Success()
    {
        // Arrange
        var result = Result.Success(10);

        // Act
        var mappedResult = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert
        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be(20);
    }

    [Fact]
    public void Result_SuccessIf_Should_Create_Conditional_Results()
    {
        // Arrange & Act
        var successResult = Result.SuccessIf(true, "Error");
        var failureResult = Result.SuccessIf(false, "Error message");

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().Be("Error message");
    }

    [Fact]
    public void Result_FailureIf_Should_Create_Conditional_Results()
    {
        // Arrange & Act
        var successResult = Result.FailureIf(false, "Error");
        var failureResult = Result.FailureIf(true, "Error message");

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().Be("Error message");
    }

    [Fact]
    public void Result_Of_Should_Handle_Synchronous_Operations()
    {
        // Arrange & Act
        var successResult = Result.Of(() => 42);
        var failureResult = Result.Of<int>(() => {
                try
                {
                    int? num = null;
                    return 1 / num.Value;
                }
                catch (Exception)
                {

                    throw new Exception("Test error");
                }
            });

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        successResult.Value.Should().Be(42);

        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().Be("Test error");
    }

    [Fact]
    public async Task Result_Of_Should_Handle_Asynchronous_Operations()
    {
        // Arrange & Act
        var successResult = await Result.Of(async () =>
        {
            await Task.Delay(1);
            return 42;
        });

        var failureResult = await Result.Of<int>(async () =>
        {
            await Task.Delay(1);
            throw new Exception("Test error");
        });

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        successResult.Value.Should().Be(42);

        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().Be("Test error");
    }

    [Fact]
    public void Result_Then_Should_Chain_Operations()
    {
        // Arrange
        var result = Result.Success(10);
        var sideEffect = 0;

        // Act
        var chainedResult = result.Then(x => sideEffect = x);

        // Assert
        chainedResult.IsSuccess.Should().BeTrue();
        sideEffect.Should().Be(10);
    }

    [Fact]
    public void Result_GetErrors_Should_Split_Error_Message()
    {
        // Arrange
        var result = Result.Failure<int>("Error1;Error2;Error3");

        // Act
        var errors = result.GetErrors();

        // Assert
        errors.Should().HaveCount(3);
        errors.Should().Contain(new[] { "Error1", "Error2", "Error3" });
    }

    [Fact]
    public void Result_ToString_Should_Format_Correctly()
    {
        // Arrange & Act
        var successResult = Result.Success(42);
        var failureResult = Result.Failure<int>("Error message");

        // Assert
        successResult.ToString().Should().Be("Success(42)");
        failureResult.ToString().Should().Be("Failure(Error message)");
    }

    [Fact]
    public void Result_Invalid_Construction_Should_Throw()
    {
        // Arrange & Act
        Action failureWithoutError = () => Result.Failure<int>(string.Empty);

        // Assert

        failureWithoutError.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot have failure result without error message.");
    }
}
