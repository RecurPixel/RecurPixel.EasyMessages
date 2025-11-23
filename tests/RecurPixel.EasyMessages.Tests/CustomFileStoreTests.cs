using System;
using System.IO;
using System.Linq;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Storage;
using Xunit;

namespace RecurPixel.EasyMessages.Tests;

public class CustomFileStoreTests
{
    public CustomFileStoreTests()
    {
        
    }

    private string? FindMessageFile()
    {
        var names = new[] { "messages/custom.json", "custom.json" };
        var dir = Directory.GetCurrentDirectory();

        for (int i = 0; i < 6; i++)
        {
            foreach (var name in names)
            {
                var candidate = Path.Combine(dir, name);
                if (File.Exists(candidate))
                    return candidate;
            }

            var parent = Path.GetDirectoryName(dir);
            if (string.IsNullOrEmpty(parent) || parent == dir)
                break;
            dir = parent;
        }

        return null;
    }

    [Fact]
    public void FileStore_ShouldLoadCustomMessages()
    {
        var path = FindMessageFile();
        Assert.False(string.IsNullOrEmpty(path), "messages/custom.json not found in repo tree.");

        MessageRegistry.Configure(new FileMessageStore(path!));

        var custom = MessageRegistry.Get("CUSTOM_001");
        Assert.Equal("Custom Notice", custom.Title);

        var auth = MessageRegistry.Get("AUTH_001");
        Assert.Contains("Authentication Failed", auth.Title);
    }
}
