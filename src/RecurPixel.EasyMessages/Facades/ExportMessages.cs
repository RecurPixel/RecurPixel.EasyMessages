using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Export messages.
    /// </summary>
    public static class Export
    {
        /// <summary>Returns EXPORT_001: Successfully exported {count} record(s) to '{fileName}'.</summary>
        public static Message Completed(string count, string fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.ImportExport.ExportCompleted);

            return message.WithParamsIfProvided(new { count, fileName });
        }

        /// <summary>Returns EXPORT_002: Failed to export data.</summary>
        public static Message Failed() =>
            MessageRegistry.Get(MessageCodes.ImportExport.ExportFailed);
    }
}
