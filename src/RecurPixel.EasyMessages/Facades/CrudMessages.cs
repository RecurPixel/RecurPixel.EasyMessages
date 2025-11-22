using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// CRUD operation messages.
    /// </summary>
    public static class Crud
    {
        /// <summary>Returns CRUD_001: {resource} has been created successfully.</summary>
        public static Message Created(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.CreatedSuccessfully);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_002: {resource} has been updated successfully.</summary>
        public static Message Updated(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.UpdatedSuccessfully);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_003: {resource} has been deleted successfully.</summary>
        public static Message Deleted(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.DeletedSuccessfully);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_004: The requested {resource} was not found.</summary>
        public static Message NotFound(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.ResourceNotFound);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_005: {resource} retrieved successfully.</summary>
        public static Message Retrieved(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.RetrievedSuccessfully);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_006: Failed to create {resource}.</summary>
        public static Message CreationFailed(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.CreationFailed);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_007: Failed to update {resource}.</summary>
        public static Message UpdateFailed(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.UpdateFailed);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_008: Failed to delete {resource}.</summary>
        public static Message DeletionFailed(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.DeletionFailed);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_009: No changes were made to {resource}.</summary>
        public static Message NoChanges(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.NoChangesDetected);

            return resource != null ? message.WithParams(new { resource }) : message;
        }

        /// <summary>Returns CRUD_010: {resource} has been modified by another user.</summary>
        public static Message Conflict(string? resource = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Crud.ConflictDetected);

            return resource != null ? message.WithParams(new { resource }) : message;
        }
    }
}
