using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using RecurPixel.EasyMessages;
using Xunit;

namespace RecurPixel.EasyMessages.Integration.Tests;

public class WebApiSampleIntegrationTests
{
    private static string FindRepoRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        for (int i = 0; i < 8; i++)
        {
            if (File.Exists(Path.Combine(dir, "RecurPixel.EasyMessages.sln")) ||
                File.Exists(Path.Combine(dir, "README.md")))
            {
                return dir;
            }

            var parent = Path.GetDirectoryName(dir);
            if (string.IsNullOrEmpty(parent) || parent == dir) break;
            dir = parent;
        }

        return Directory.GetCurrentDirectory();
    }

    [Fact]
    public async Task MessageEndpoint_ReturnsCustomAuthMessage()
    {
    }
}
