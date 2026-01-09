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

        var apiResponse = Assert.IsType<ApiResponse>(obj.Value);
        Assert.Equal("X_001", apiResponse.Code);
        Assert.Equal("T", apiResponse.Title);
        Assert.False(apiResponse.Success);
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
        var obj = Assert.IsAssignableFrom<ObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse>(obj.Value);

        Assert.True(apiResponse.Success);
        Assert.Equal("success", apiResponse.Type);
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

    [Fact]
    public void ToCreated_ReturnsCreatedResultWithLocation()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Resource Created",
            Description = "Successfully created",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_004");
        var location = "/api/resource/123";

        var result = message.ToCreated(location);

        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(location, createdResult.Location);
        Assert.IsType<ApiResponse>(createdResult.Value);
    }

    [Fact]
    public void ToCreated_ThrowsArgumentNullException_WhenLocationIsNull()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Resource Created",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_005");

        Assert.Throws<ArgumentNullException>(() => message.ToCreated(null!));
    }

    [Fact]
    public void ToCreated_ThrowsArgumentNullException_WhenLocationIsEmpty()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Resource Created",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_006");

        Assert.Throws<ArgumentNullException>(() => message.ToCreated(string.Empty));
    }

    [Fact]
    public void ToCreatedAtAction_ReturnsCreatedAtActionResult_WithActionName()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Resource Created",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_007");

        var result = message.ToCreatedAtAction("GetResource");

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetResource", createdAtActionResult.ActionName);
        Assert.Null(createdAtActionResult.ControllerName);
        Assert.Null(createdAtActionResult.RouteValues);
        Assert.IsType<ApiResponse>(createdAtActionResult.Value);
    }

    [Fact]
    public void ToCreatedAtAction_ReturnsCreatedAtActionResult_WithAllParameters()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Resource Created",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_008");
        var routeValues = new { id = 123 };

        var result = message.ToCreatedAtAction("GetResource", "Resources", routeValues);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetResource", createdAtActionResult.ActionName);
        Assert.Equal("Resources", createdAtActionResult.ControllerName);
        Assert.NotNull(createdAtActionResult.RouteValues);
        Assert.IsType<ApiResponse>(createdAtActionResult.Value);
    }

    [Fact]
    public void ToCreatedAtAction_ThrowsArgumentNullException_WhenActionNameIsNull()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Resource Created",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_009");

        Assert.Throws<ArgumentNullException>(() => message.ToCreatedAtAction(null!));
    }

    [Fact]
    public void ToCreatedAtAction_ThrowsArgumentNullException_WhenActionNameIsEmpty()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Resource Created",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_010");

        Assert.Throws<ArgumentNullException>(() => message.ToCreatedAtAction(string.Empty));
    }

    [Fact]
    public void ToNoContent_ReturnsNoContentResult()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Deleted",
            HttpStatusCode = 204,
        };

        var message = template.ToMessage("X_011");

        var result = message.ToNoContent();

        Assert.IsType<NoContentResult>(result);
    }

    [Theory]
    [InlineData(200, typeof(OkObjectResult))]
    [InlineData(201, typeof(CreatedResult))]
    [InlineData(204, typeof(NoContentResult))]
    [InlineData(400, typeof(BadRequestObjectResult))]
    [InlineData(401, typeof(UnauthorizedObjectResult))]
    [InlineData(404, typeof(NotFoundObjectResult))]
    [InlineData(422, typeof(UnprocessableEntityObjectResult))]
    public void ToApiResponse_ReturnsCorrectResultType_ForCommonStatusCodes(int statusCode, Type expectedType)
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Test",
            HttpStatusCode = statusCode,
        };

        var message = template.ToMessage("X_012");

        var result = message.ToApiResponse();

        Assert.IsType(expectedType, result);
    }

    [Fact]
    public void ToApiResponse_ReturnsForbiddenObjectResult_ForStatusCode403()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = "Forbidden",
            HttpStatusCode = 403,
        };

        var message = template.ToMessage("X_013");

        var result = message.ToApiResponse();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, objectResult.StatusCode);
        Assert.IsType<ApiResponse>(objectResult.Value);
    }

    [Fact]
    public void ToCreated_ReturnsApiResponseWithCorrectProperties()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Created",
            Description = "Resource created successfully",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_014");
        var location = "/api/resources/456";

        var result = message.ToCreated(location);

        var createdResult = Assert.IsType<CreatedResult>(result);
        var apiResponse = Assert.IsType<ApiResponse>(createdResult.Value);
        Assert.Equal("X_014", apiResponse.Code);
        Assert.Equal("Created", apiResponse.Title);
        Assert.True(apiResponse.Success);
    }

    [Fact]
    public void ToCreatedAtAction_ReturnsApiResponseWithCorrectProperties()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Success,
            Title = "Created",
            Description = "Resource created successfully",
            HttpStatusCode = 201,
        };

        var message = template.ToMessage("X_015");

        var result = message.ToCreatedAtAction("GetById", "Resources", new { id = 789 });

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var apiResponse = Assert.IsType<ApiResponse>(createdAtActionResult.Value);
        Assert.Equal("X_015", apiResponse.Code);
        Assert.Equal("Created", apiResponse.Title);
        Assert.True(apiResponse.Success);
    }

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(422)]
    [InlineData(500)]
    public void ToApiResponse_PreservesMessageProperties(int statusCode)
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Error,
            Title = $"Test {statusCode}",
            Description = "Test description",
            HttpStatusCode = statusCode,
        };

        var message = template.ToMessage($"X_{statusCode}");

        var result = message.ToApiResponse();

        // For NoContentResult, there's no Value property
        if (statusCode == 204)
        {
            Assert.IsType<NoContentResult>(result);
            return;
        }

        var objectResult = result as ObjectResult;
        Assert.NotNull(objectResult);

        var apiResponse = Assert.IsType<ApiResponse>(objectResult.Value);
        Assert.Equal($"X_{statusCode}", apiResponse.Code);
        Assert.Equal($"Test {statusCode}", apiResponse.Title);
        Assert.Equal("Test description", apiResponse.Description);
        Assert.Equal("error", apiResponse.Type);
        Assert.False(apiResponse.Success);
    }

    [Fact]
    public void ToApiResponse_MapsAllMessageFieldsToApiResponse()
    {
        Dispose();
        var template = new MessageTemplate
        {
            Type = MessageType.Info,
            Title = "Info Message",
            Description = "Information details",
            HttpStatusCode = 200,
        };

        var message = template
            .ToMessage("X_016")
            .WithData(new { Count = 42 })
            .WithCorrelationId("corr-123")
            .WithMetadata("key1", "value1");

        var result = message.ToApiResponse();
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse>(objectResult.Value);

        Assert.Equal("X_016", apiResponse.Code);
        Assert.Equal("Info Message", apiResponse.Title);
        Assert.Equal("Information details", apiResponse.Description);
        Assert.Equal("info", apiResponse.Type);
        Assert.True(apiResponse.Success); // Info is considered success
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("corr-123", apiResponse.CorrelationId);
        Assert.NotNull(apiResponse.Metadata);
        Assert.Contains("key1", apiResponse.Metadata.Keys);
    }
}
