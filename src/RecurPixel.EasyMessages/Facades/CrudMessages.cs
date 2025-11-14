using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Core.Extensions;

namespace RecurPixel.EasyMessages.Facades;

public class CrudMessages
{
    public static Message Created(string resource) =>
        MessageRegistry.Get("CRUD_001").WithParams(new { resource });
    
    public static Message Updated(string resource) =>
        MessageRegistry.Get("CRUD_002").WithParams(new { resource });
    
    public static Message Deleted(string resource) =>
        MessageRegistry.Get("CRUD_003").WithParams(new { resource });
    
    public static Message NotFound(string resource) =>
        MessageRegistry.Get("CRUD_004").WithParams(new { resource });
}