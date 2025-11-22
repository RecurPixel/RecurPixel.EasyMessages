using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Import and export messages.
    /// </summary>
    public static class Import
    {
        /// <summary>Returns IMPORT_001: Successfully imported {count} record(s).</summary>
        public static Message Completed(string? count)
        {
            var message = MessageRegistry.Get(MessageCodes.ImportExport.ImportCompleted);

            return count != null ? message.WithParams(new { count }) : message;
        }

        /// <summary>Returns IMPORT_002: Failed to import data from '{fileName}'.</summary>
        public static Message Failed(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.ImportExport.ImportFailed);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns IMPORT_003: Imported {successCount} of {totalCount} records.</summary>
        public static Message Partial(string? successCount, string? totalCount)
        {
            var message = MessageRegistry.Get(MessageCodes.ImportExport.PartialImport);

            return message.WithParamsIfProvided(new { successCount, totalCount });
        }
    }
}
