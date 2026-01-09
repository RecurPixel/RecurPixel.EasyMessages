using FluentAssertions;
using RecurPixel.EasyMessages.AspNetCore.Configuration;

namespace RecurPixel.EasyMessages.Tests.Unit.Configuration;

public class StorageOptionsTests
{
    [Fact]
    public void Constructor_ShouldHaveNullDefaults()
    {
        // Act
        var options = new StorageOptions();

        // Assert
        options.CustomMessagesPath.Should().BeNull();
        options.CustomStorePaths.Should().BeNull();
    }

    [Fact]
    public void CustomMessagesPath_ShouldBeSettable()
    {
        // Arrange
        var options = new StorageOptions();
        const string path = "messages/custom.json";

        // Act
        options.CustomMessagesPath = path;

        // Assert
        options.CustomMessagesPath.Should().Be(path);
    }

    [Fact]
    public void CustomStorePaths_ShouldSupportMultiplePaths()
    {
        // Arrange
        var options = new StorageOptions();
        var paths = new List<string>
        {
            "messages/auth.json",
            "messages/payment.json",
            "messages/domain.json"
        };

        // Act
        options.CustomStorePaths = paths;

        // Assert
        options.CustomStorePaths.Should().BeEquivalentTo(paths);
        options.CustomStorePaths.Should().HaveCount(3);
    }

    [Fact]
    public void CustomStorePaths_CanBeEmpty()
    {
        // Arrange
        var options = new StorageOptions
        {
            CustomStorePaths = new List<string>()
        };

        // Assert
        options.CustomStorePaths.Should().NotBeNull();
        options.CustomStorePaths.Should().BeEmpty();
    }
}
