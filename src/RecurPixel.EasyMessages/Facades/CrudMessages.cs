using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    public static class Crud
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
}
