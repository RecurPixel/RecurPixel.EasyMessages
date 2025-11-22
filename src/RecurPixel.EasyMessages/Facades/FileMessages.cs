using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// File operation messages.
    /// </summary>
    public static class File
    {
        /// <summary>Returns FILE_001: '{fileName}' has been uploaded successfully.</summary>
        public static Message UploadSuccessful(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.FileUploadedSuccessfully);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns FILE_002: Only {allowedTypes} files are allowed.</summary>
        public static Message InvalidType(string? allowedTypes)
        {
            var message = MessageRegistry.Get(MessageCodes.File.InvalidFileType);

            return allowedTypes != null ? message.WithParams(new { allowedTypes }) : message;
        }

        /// <summary>Returns FILE_003: Maximum file size is {maxSize}.</summary>
        public static Message TooLarge(string? maxSize)
        {
            var message = MessageRegistry.Get(MessageCodes.File.FileTooLarge);

            return maxSize != null ? message.WithParams(new { maxSize }) : message;
        }

        /// <summary>Returns FILE_004: Failed to upload '{fileName}'.</summary>
        public static Message UploadFailed(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.FileUploadFailed);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns FILE_005: '{fileName}' has been downloaded successfully.</summary>
        public static Message DownloadSuccessful(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.FileDownloadedSuccessfully);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns FILE_006: The requested file '{fileName}' was not found.</summary>
        public static Message NotFound(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.FileNotFound);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns FILE_007: You don't have permission to access '{fileName}'.</summary>
        public static Message AccessDenied(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.FileAccessDenied);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns FILE_008: '{fileName}' has been deleted successfully.</summary>
        public static Message DeletedSuccessfully(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.FileDeletedSuccessfully);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns FILE_009: The file '{fileName}' appears to be corrupted.</summary>
        public static Message Corrupted(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.CorruptedFile);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns FILE_010: You have exceeded your storage quota.</summary>
        public static Message QuotaExceeded() => MessageRegistry.Get(MessageCodes.File.StorageQuotaExceeded);

        /// <summary>Returns FILE_011: A file named '{fileName}' already exists.</summary>
        public static Message AlreadyExists(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.FileAlreadyExists);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }

        /// <summary>Returns FILE_012: The file '{fileName}' contains malicious content and has been rejected.</summary>
        public static Message VirusDetected(string? fileName)
        {
            var message = MessageRegistry.Get(MessageCodes.File.VirusDetected);

            return fileName != null ? message.WithParams(new { fileName }) : message;
        }
    }
}
