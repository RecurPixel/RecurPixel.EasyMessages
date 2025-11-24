using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;
using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class MessageResultExtensionsTests : UnitTestBase
{
    [Fact]
    public void ToApiResponse_MapsFieldsAndStatusCode()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = "T",
            Description = "D",
            HttpStatusCode = 418,
        };

        var message = template.ToMessage("X_001");

        var result = message.ToApiResponse();

        Assert.IsType<ObjectResult>(result);
        var obj = (ObjectResult)result;
        Assert.Equal(418, obj.StatusCode);

        var api = Assert.IsType<ApiResponse>(obj.Value);
        Assert.Equal("X_001", api.Code);
        Assert.Equal("T", api.Title);
        Assert.False(api.Success);
    }

    [Fact]
    public void ToApiResponse_SetsSuccessFlagForSuccessTypes()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "OK",
            Description = "All good",
            HttpStatusCode = 200,
        };

        var message = template.ToMessage("X_002");

        var result = message.ToApiResponse();
        var obj = Assert.IsType<ObjectResult>(result);
        var api = Assert.IsType<ApiResponse>(obj.Value);

        Assert.True(api.Success);
        Assert.Equal(message.Type.ToString().ToLowerInvariant(), api.Type); // type should match message type
    }

    [Fact]
    public void ToMinimalApiResult_Returns_IResult()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "OK",
            Description = "All good",
            HttpStatusCode = 200,
        };

        var message = template.ToMessage("X_003");

        var result = message.ToMinimalApiResult();

        Assert.NotNull(result);
        // IResult implementations for Results.Json are internal; ensure the returned object is an IResult.
        Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(result);
    }
}
