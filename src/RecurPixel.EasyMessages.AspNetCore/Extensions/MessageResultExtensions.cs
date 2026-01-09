using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;

namespace RecurPixel.EasyMessages.AspNetCore;

public static class MessageResultExtensions
{
    /// <summary>
    /// Cache for MessageType enum to lowercase string conversions to avoid repeated allocations
    /// </summary>
    private static class MessageTypeStringCache
    {
        // Using a frozen dictionary for even better read performance on .NET 8+
        private static readonly Dictionary<MessageType, string> Cache = new()
        {
            [MessageType.Success] = "success",
            [MessageType.Error] = "error",
            [MessageType.Warning] = "warning",
            [MessageType.Info] = "info",
        };

        public static string GetTypeName(MessageType type) => Cache[type];
    }

    /// <summary>
    /// Shared empty dictionary to avoid allocations when metadata is null
    /// </summary>
    private static readonly Dictionary<string, object> EmptyMetadata = new(0);

    /// <summary>
    /// Converts Message to ApiResponse
    /// </summary>
    private static ApiResponse ToApiResponseModel(this Message message)
    {
        return new ApiResponse
        {
            Success = message.Type is MessageType.Success or MessageType.Info,
            Code = message.Code,
            Type = MessageTypeStringCache.GetTypeName(message.Type),
            Title = message.Title,
            Description = message.Description,
            Data = message.Data,
            Timestamp = message.Timestamp,
            CorrelationId = message.CorrelationId,
            // Only include metadata if it has items, otherwise use null to reduce JSON size
            Metadata = message.Metadata?.Count > 0 ? message.Metadata : null,
        };
    }

    /// <summary>
    /// Converts message to IActionResult with appropriate HTTP status code
    /// </summary>
    public static IActionResult ToApiResponse(this Message message)
    {
        var response = message.ToApiResponseModel();

        return message.HttpStatusCode switch
        {
            200 => new OkObjectResult(response),
            201 => new CreatedResult(string.Empty, response),
            204 => new NoContentResult(),
            400 => new BadRequestObjectResult(response),
            401 => new UnauthorizedObjectResult(response),
            403 => new ObjectResult(response) { StatusCode = 403 },
            404 => new NotFoundObjectResult(response),
            422 => new UnprocessableEntityObjectResult(response),
            _ => new ObjectResult(response) { StatusCode = message.HttpStatusCode },
        };
    }

    /// <summary>
    /// Returns 201 Created with Location header
    /// </summary>
    public static IActionResult ToCreated(this Message message, string location)
    {
        if (string.IsNullOrEmpty(location))
            throw new ArgumentNullException(nameof(location));

        var response = message.ToApiResponseModel();
        return new CreatedResult(location, response);
    }

    /// <summary>
    /// Returns 201 Created using MVC action routing
    /// </summary>
    public static IActionResult ToCreatedAtAction(
        this Message message,
        string actionName,
        string? controllerName = null,
        object? routeValues = null
    )
    {
        if (string.IsNullOrEmpty(actionName))
            throw new ArgumentNullException(nameof(actionName));

        var response = message.ToApiResponseModel();
        return new CreatedAtActionResult(actionName, controllerName, routeValues, response);
    }

    /// <summary>
    /// Returns 204 No Content (typically for DELETE or PUT with no response body)
    /// </summary>
    public static IActionResult ToNoContent(this Message message)
    {
        return new NoContentResult();
    }

    /// <summary>
    /// Converts message to IResult for Minimal APIs
    /// </summary>
    public static IResult ToMinimalApiResult(this Message message)
    {
        var response = message.ToApiResponseModel();

        return message.HttpStatusCode switch
        {
            200 => Results.Ok(response),
            201 => Results.Created(string.Empty, response),
            204 => Results.NoContent(),
            400 => Results.BadRequest(response),
            401 => Results.Json(response, statusCode: 401), // Include response body
            403 => Results.Json(response, statusCode: 403), // Include response body
            404 => Results.NotFound(response),
            422 => Results.UnprocessableEntity(response),
            _ => Results.Json(response, statusCode: message.HttpStatusCode),
        };
    }
}
