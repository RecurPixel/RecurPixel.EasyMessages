using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using Microsoft.AspNetCore.Http.Abstractions;

namespace RecurPixel.EasyMessages.AspNetCore;

public static class MessageResultExtensions
{
    public static IActionResult ToApiResponse(this Message message)
    {
        var response = new ApiResponse
        {
            Success = message.Type is MessageType.Success or MessageType.Info,
            Code = message.Code,
            Type = message.Type.ToString().ToLowerInvariant(),
            Title = message.Title,
            Description = message.Description,
            Data = message.Data,
            Timestamp = message.Timestamp,
            CorrelationId = message.CorrelationId,
            Metadata = message.Metadata
        };
        
        return new ObjectResult(response)
        {
            StatusCode = message.HttpStatusCode
        };
    }
    
    public static IResult ToMinimalApiResult(this Message message)
    {
        var response = new ApiResponse
        {
            Success = message.Type is MessageType.Success or MessageType.Info,
            Code = message.Code,
            Type = message.Type.ToString().ToLowerInvariant(),
            Title = message.Title,
            Description = message.Description,
            Data = message.Data,
            Timestamp = message.Timestamp,
            CorrelationId = message.CorrelationId,
            Metadata = message.Metadata
        };
        
        return Results.Json(response, statusCode: message.HttpStatusCode);
    }
}