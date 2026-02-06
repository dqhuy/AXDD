using AXDD.BuildingBlocks.Common.Extensions;

namespace AXDD.Tests.Unit;

public class StringExtensionsTests
{
    [Fact]
    public void ToSlug_ShouldConvertToLowerCaseAndReplaceSpaces()
    {
        // Arrange
        var input = "Hello World";

        // Act
        var result = input.ToSlug();

        // Assert
        Assert.Equal("hello-world", result);
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnTrue_WhenStringIsEmpty()
    {
        // Arrange
        var input = "";

        // Act
        var result = input.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnFalse_WhenStringHasValue()
    {
        // Arrange
        var input = "test";

        // Act
        var result = input.IsNullOrEmpty();

        // Assert
        Assert.False(result);
    }
}
