using Xunit;
using FluentAssertions;
using Wrapture.Specifications;

namespace Wrapture.Tests.Specifications;

public class SpecificationTests
{
    [Fact]
    public void Specification_Should_Satisfy_Condition()
    {
        // Arrange
        var spec = Specification<int>.All.And(x => x > 10);

        // Act & Assert
        spec.IsSatisfiedBy(20).Should().BeTrue();
        spec.IsSatisfiedBy(5).Should().BeFalse();
    }

    [Fact]
    public void Specification_Should_Combine_With_And()
    {
        // Arrange
        var spec1 = Specification<int>.All.And(x => x > 10);
        var spec2 = Specification<int>.All.And(x => x < 20);
        var combinedSpec = spec1.And(spec2);

        // Act & Assert
        combinedSpec.IsSatisfiedBy(15).Should().BeTrue();
        combinedSpec.IsSatisfiedBy(25).Should().BeFalse();
    }
}
