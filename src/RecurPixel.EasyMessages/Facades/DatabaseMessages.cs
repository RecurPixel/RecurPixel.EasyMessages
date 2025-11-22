using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Database operation messages.
    /// </summary>
    public static class Database
    {
        /// <summary>Returns DB_001: Unable to connect to the database.</summary>
        public static Message ConnectionFailed() =>
            MessageRegistry.Get(MessageCodes.Db.DatabaseConnectionFailed);

        /// <summary>Returns DB_002: {resource} with this {field} already exists.</summary>
        public static Message DuplicateEntry(string? resource, string? field)
        {
            var message = MessageRegistry.Get(MessageCodes.Db.DuplicateEntry);

            return message.WithParamsIfProvided(new { resource, field });
        }

        /// <summary>Returns DB_003: Cannot perform this operation due to related records.</summary>
        public static Message ForeignKeyConstraint() =>
            MessageRegistry.Get(MessageCodes.Db.ForeignKeyConstraint);

        /// <summary>Returns DB_004: The database transaction failed and has been rolled back.</summary>
        public static Message TransactionFailed() =>
            MessageRegistry.Get(MessageCodes.Db.TransactionFailed);

        /// <summary>Returns DB_005: A data integrity error occurred.</summary>
        public static Message IntegrityError() =>
            MessageRegistry.Get(MessageCodes.Db.DataIntegrityError);

        /// <summary>Returns DB_006: The database query took too long to execute.</summary>
        public static Message QueryTimeout() => MessageRegistry.Get(MessageCodes.Db.QueryTimeout);

        /// <summary>Returns DB_007: A database deadlock was detected. Please try again.</summary>
        public static Message Deadlock() => MessageRegistry.Get(MessageCodes.Db.DeadlockDetected);

        /// <summary>Returns DB_008: Database migrations are pending. Some features may not work correctly.</summary>
        public static Message MigrationPending() =>
            MessageRegistry.Get(MessageCodes.Db.MigrationPending);
    }
}
