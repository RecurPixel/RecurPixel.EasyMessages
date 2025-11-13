namespace RecurPixel.EasyMessages.Core;

/// <summary>
/// Standard message codes for RecurPixel.EasyMessages.
/// Use these constants instead of magic strings for type safety and IntelliSense support.
/// </summary>
public static class MessageCodes
{
    #region Authentication & Authorization (AUTH)

    /// <summary>AUTH_001: Invalid username or password.</summary>
    public const string AuthenticationFailed = "AUTH_001";
    
    /// <summary>AUTH_002: You don't have permission to access this resource.</summary>
    public const string UnauthorizedAccess = "AUTH_002";
    
    /// <summary>AUTH_003: Welcome back!</summary>
    public const string LoginSuccessful = "AUTH_003";
    
    /// <summary>AUTH_004: Your session has expired. Please log in again.</summary>
    public const string SessionExpired = "AUTH_004";
    
    /// <summary>AUTH_005: The authentication token is invalid or has expired.</summary>
    public const string InvalidToken = "AUTH_005";
    
    /// <summary>AUTH_006: Your account has been locked due to multiple failed login attempts.</summary>
    public const string AccountLocked = "AUTH_006";
    
    /// <summary>AUTH_007: You have been successfully logged out.</summary>
    public const string LogoutSuccessful = "AUTH_007";
    
    /// <summary>AUTH_008: You must reset your password before continuing.</summary>
    public const string PasswordResetRequired = "AUTH_008";
    
    /// <summary>AUTH_009: Unable to refresh your session. Please log in again.</summary>
    public const string InvalidRefreshToken = "AUTH_009";
    
    /// <summary>AUTH_010: Please complete multi-factor authentication to continue.</summary>
    public const string MfaRequired = "AUTH_010";

    #endregion

    #region CRUD Operations (CRUD)

    /// <summary>CRUD_001: {resource} has been created successfully.</summary>
    public const string CreatedSuccessfully = "CRUD_001";
    
    /// <summary>CRUD_002: {resource} has been updated successfully.</summary>
    public const string UpdatedSuccessfully = "CRUD_002";
    
    /// <summary>CRUD_003: {resource} has been deleted successfully.</summary>
    public const string DeletedSuccessfully = "CRUD_003";
    
    /// <summary>CRUD_004: The requested {resource} was not found.</summary>
    public const string ResourceNotFound = "CRUD_004";
    
    /// <summary>CRUD_005: {resource} retrieved successfully.</summary>
    public const string RetrievedSuccessfully = "CRUD_005";
    
    /// <summary>CRUD_006: Failed to create {resource}.</summary>
    public const string CreationFailed = "CRUD_006";
    
    /// <summary>CRUD_007: Failed to update {resource}.</summary>
    public const string UpdateFailed = "CRUD_007";
    
    /// <summary>CRUD_008: Failed to delete {resource}.</summary>
    public const string DeletionFailed = "CRUD_008";
    
    /// <summary>CRUD_009: No changes were made to {resource}.</summary>
    public const string NoChangesDetected = "CRUD_009";
    
    /// <summary>CRUD_010: {resource} has been modified by another user.</summary>
    public const string ConflictDetected = "CRUD_010";

    #endregion

    #region Validation (VAL)

    /// <summary>VAL_001: Please check your input and try again.</summary>
    public const string ValidationFailed = "VAL_001";
    
    /// <summary>VAL_002: The field '{field}' is required.</summary>
    public const string RequiredFieldMissing = "VAL_002";
    
    /// <summary>VAL_003: The field '{field}' has an invalid format.</summary>
    public const string InvalidFormat = "VAL_003";
    
    /// <summary>VAL_004: The field '{field}' must be between {min} and {max}.</summary>
    public const string ValueOutOfRange = "VAL_004";
    
    /// <summary>VAL_005: Please provide a valid email address.</summary>
    public const string InvalidEmail = "VAL_005";
    
    /// <summary>VAL_006: Please provide a valid phone number.</summary>
    public const string InvalidPhoneNumber = "VAL_006";
    
    /// <summary>VAL_007: Password must meet security requirements.</summary>
    public const string PasswordTooWeak = "VAL_007";
    
    /// <summary>VAL_008: Password and confirmation password must match.</summary>
    public const string PasswordsDontMatch = "VAL_008";
    
    /// <summary>VAL_009: The field '{field}' contains an invalid date.</summary>
    public const string InvalidDate = "VAL_009";
    
    /// <summary>VAL_010: The field '{field}' must be at least {minLength} characters long.</summary>
    public const string ValueTooShort = "VAL_010";
    
    /// <summary>VAL_011: The field '{field}' must not exceed {maxLength} characters.</summary>
    public const string ValueTooLong = "VAL_011";
    
    /// <summary>VAL_012: Please provide a valid URL.</summary>
    public const string InvalidUrl = "VAL_012";
    
    /// <summary>VAL_013: The field '{field}' must be a valid {type}.</summary>
    public const string InvalidFileExtension = "VAL_013";
    
    /// <summary>VAL_014: The value '{value}' already exists.</summary>
    public const string DuplicateValue = "VAL_014";
    
    /// <summary>VAL_015: The field '{field}' contains invalid characters.</summary>
    public const string InvalidCharacters = "VAL_015";

    #endregion

    #region System Messages (SYS)

    /// <summary>SYS_001: An unexpected error occurred. Please try again later.</summary>
    public const string SystemError = "SYS_001";
    
    /// <summary>SYS_002: Your request is being processed. Please wait...</summary>
    public const string ProcessingRequest = "SYS_002";
    
    /// <summary>SYS_003: Some features may be temporarily unavailable.</summary>
    public const string ServiceDegraded = "SYS_003";
    
    /// <summary>SYS_004: The system is currently under maintenance.</summary>
    public const string MaintenanceMode = "SYS_004";
    
    /// <summary>SYS_005: The operation completed successfully.</summary>
    public const string OperationCompleted = "SYS_005";
    
    /// <summary>SYS_006: You have exceeded the rate limit.</summary>
    public const string RateLimitExceeded = "SYS_006";
    
    /// <summary>SYS_007: The service is temporarily unavailable.</summary>
    public const string ServiceUnavailable = "SYS_007";
    
    /// <summary>SYS_008: Your request has been queued for processing.</summary>
    public const string RequestQueued = "SYS_008";
    
    /// <summary>SYS_009: The request timed out. Please try again.</summary>
    public const string Timeout = "SYS_009";
    
    /// <summary>SYS_010: A system configuration error occurred.</summary>
    public const string ConfigurationError = "SYS_010";

    #endregion

    #region Database Operations (DB)

    /// <summary>DB_001: Unable to connect to the database.</summary>
    public const string DatabaseConnectionFailed = "DB_001";
    
    /// <summary>DB_002: {resource} with this {field} already exists.</summary>
    public const string DuplicateEntry = "DB_002";
    
    /// <summary>DB_003: Cannot perform this operation due to related records.</summary>
    public const string ForeignKeyConstraint = "DB_003";
    
    /// <summary>DB_004: The database transaction failed and has been rolled back.</summary>
    public const string TransactionFailed = "DB_004";
    
    /// <summary>DB_005: A data integrity error occurred.</summary>
    public const string DataIntegrityError = "DB_005";
    
    /// <summary>DB_006: The database query took too long to execute.</summary>
    public const string QueryTimeout = "DB_006";
    
    /// <summary>DB_007: A database deadlock was detected. Please try again.</summary>
    public const string DeadlockDetected = "DB_007";
    
    /// <summary>DB_008: Database migrations are pending. Some features may not work correctly.</summary>
    public const string MigrationPending = "DB_008";

    #endregion

    #region File Operations (FILE)

    /// <summary>FILE_001: '{fileName}' has been uploaded successfully.</summary>
    public const string FileUploadedSuccessfully = "FILE_001";
    
    /// <summary>FILE_002: Only {allowedTypes} files are allowed.</summary>
    public const string InvalidFileType = "FILE_002";
    
    /// <summary>FILE_003: Maximum file size is {maxSize}.</summary>
    public const string FileTooLarge = "FILE_003";
    
    /// <summary>FILE_004: Failed to upload '{fileName}'.</summary>
    public const string FileUploadFailed = "FILE_004";
    
    /// <summary>FILE_005: '{fileName}' has been downloaded successfully.</summary>
    public const string FileDownloadedSuccessfully = "FILE_005";
    
    /// <summary>FILE_006: The requested file '{fileName}' was not found.</summary>
    public const string FileNotFound = "FILE_006";
    
    /// <summary>FILE_007: You don't have permission to access '{fileName}'.</summary>
    public const string FileAccessDenied = "FILE_007";
    
    /// <summary>FILE_008: '{fileName}' has been deleted successfully.</summary>
    public const string FileDeletedSuccessfully = "FILE_008";
    
    /// <summary>FILE_009: The file '{fileName}' appears to be corrupted.</summary>
    public const string CorruptedFile = "FILE_009";
    
    /// <summary>FILE_010: You have exceeded your storage quota.</summary>
    public const string StorageQuotaExceeded = "FILE_010";
    
    /// <summary>FILE_011: A file named '{fileName}' already exists.</summary>
    public const string FileAlreadyExists = "FILE_011";
    
    /// <summary>FILE_012: The file '{fileName}' contains malicious content and has been rejected.</summary>
    public const string VirusDetected = "FILE_012";

    #endregion

    #region Network & API (NET)

    /// <summary>NET_001: A network error occurred. Please check your connection.</summary>
    public const string NetworkError = "NET_001";
    
    /// <summary>NET_002: The request timed out. Please try again.</summary>
    public const string RequestTimeout = "NET_002";
    
    /// <summary>NET_003: The server could not understand your request.</summary>
    public const string BadRequest = "NET_003";
    
    /// <summary>NET_004: The server encountered an error. Please try again later.</summary>
    public const string ServerError = "NET_004";
    
    /// <summary>NET_005: You have exceeded the API rate limit.</summary>
    public const string ApiRateLimitExceeded = "NET_005";
    
    /// <summary>NET_006: Unable to connect to the server.</summary>
    public const string ConnectionRefused = "NET_006";
    
    /// <summary>NET_007: The server's SSL certificate is invalid or expired.</summary>
    public const string SslCertificateError = "NET_007";
    
    /// <summary>NET_008: Your connection is slower than usual. This may affect performance.</summary>
    public const string SlowConnection = "NET_008";
    
    /// <summary>NET_009: The gateway did not receive a timely response from the upstream server.</summary>
    public const string GatewayTimeout = "NET_009";
    
    /// <summary>NET_010: Successfully connected to {service}.</summary>
    public const string ConnectionEstablished = "NET_010";

    #endregion

    #region Payment & Transactions (PAY)

    /// <summary>PAY_001: Your payment of {amount} has been processed successfully.</summary>
    public const string PaymentSuccessful = "PAY_001";
    
    /// <summary>PAY_002: Your payment could not be processed.</summary>
    public const string PaymentFailed = "PAY_002";
    
    /// <summary>PAY_003: Your account has insufficient funds to complete this transaction.</summary>
    public const string InsufficientFunds = "PAY_003";
    
    /// <summary>PAY_004: Your card was declined by the issuing bank.</summary>
    public const string CardDeclined = "PAY_004";
    
    /// <summary>PAY_005: The card details provided are invalid.</summary>
    public const string InvalidCardDetails = "PAY_005";
    
    /// <summary>PAY_006: The card has expired. Please use a different payment method.</summary>
    public const string CardExpired = "PAY_006";
    
    /// <summary>PAY_007: Your refund of {amount} has been processed.</summary>
    public const string RefundProcessed = "PAY_007";
    
    /// <summary>PAY_008: Unable to process your refund request.</summary>
    public const string RefundFailed = "PAY_008";
    
    /// <summary>PAY_009: Your payment is being processed. This may take a few minutes.</summary>
    public const string PaymentPending = "PAY_009";
    
    /// <summary>PAY_010: This transaction exceeds your daily limit of {limit}.</summary>
    public const string TransactionLimitExceeded = "PAY_010";
    
    /// <summary>PAY_011: The payment gateway is temporarily unavailable.</summary>
    public const string PaymentGatewayError = "PAY_011";
    
    /// <summary>PAY_012: Your subscription has been activated successfully.</summary>
    public const string SubscriptionActivated = "PAY_012";
    
    /// <summary>PAY_013: Your subscription will expire on {expiryDate}.</summary>
    public const string SubscriptionExpiringSoon = "PAY_013";
    
    /// <summary>PAY_014: Your subscription has been cancelled.</summary>
    public const string SubscriptionCancelled = "PAY_014";

    #endregion

    #region Email & Notifications (EMAIL)

    /// <summary>EMAIL_001: Your email has been sent to {recipient}.</summary>
    public const string EmailSentSuccessfully = "EMAIL_001";
    
    /// <summary>EMAIL_002: Failed to send email to {recipient}.</summary>
    public const string EmailDeliveryFailed = "EMAIL_002";
    
    /// <summary>EMAIL_003: Your email address has been verified successfully.</summary>
    public const string EmailVerified = "EMAIL_003";
    
    /// <summary>EMAIL_004: The email verification link is invalid or has expired.</summary>
    public const string InvalidVerificationLink = "EMAIL_004";
    
    /// <summary>EMAIL_005: A verification email has been sent to {email}.</summary>
    public const string VerificationEmailSent = "EMAIL_005";

    #endregion

    #region Search & Filter (SEARCH)

    /// <summary>SEARCH_001: No results found for '{query}'.</summary>
    public const string NoResultsFound = "SEARCH_001";
    
    /// <summary>SEARCH_002: Found {count} result(s) for '{query}'.</summary>
    public const string SearchCompleted = "SEARCH_002";
    
    /// <summary>SEARCH_003: Your search returned too many results.</summary>
    public const string TooManyResults = "SEARCH_003";
    
    /// <summary>SEARCH_004: The search query contains invalid characters or syntax.</summary>
    public const string InvalidSearchQuery = "SEARCH_004";

    #endregion

    #region Import/Export (IMPORT/EXPORT)

    /// <summary>IMPORT_001: Successfully imported {count} record(s).</summary>
    public const string ImportCompleted = "IMPORT_001";
    
    /// <summary>IMPORT_002: Failed to import data from '{fileName}'.</summary>
    public const string ImportFailed = "IMPORT_002";
    
    /// <summary>IMPORT_003: Imported {successCount} of {totalCount} records.</summary>
    public const string PartialImport = "IMPORT_003";
    
    /// <summary>EXPORT_001: Successfully exported {count} record(s) to '{fileName}'.</summary>
    public const string ExportCompleted = "EXPORT_001";
    
    /// <summary>EXPORT_002: Failed to export data.</summary>
    public const string ExportFailed = "EXPORT_002";

    #endregion

    #region Category Groups (For Documentation/Organization)

    /// <summary>All authentication and authorization message codes.</summary>
    public static class Auth
    {
        public const string AuthenticationFailed = MessageCodes.AuthenticationFailed;
        public const string UnauthorizedAccess = MessageCodes.UnauthorizedAccess;
        public const string LoginSuccessful = MessageCodes.LoginSuccessful;
        public const string SessionExpired = MessageCodes.SessionExpired;
        public const string InvalidToken = MessageCodes.InvalidToken;
        public const string AccountLocked = MessageCodes.AccountLocked;
        public const string LogoutSuccessful = MessageCodes.LogoutSuccessful;
        public const string PasswordResetRequired = MessageCodes.PasswordResetRequired;
        public const string InvalidRefreshToken = MessageCodes.InvalidRefreshToken;
        public const string MfaRequired = MessageCodes.MfaRequired;
    }

    /// <summary>All CRUD operation message codes.</summary>
    public static class Crud
    {
        public const string CreatedSuccessfully = MessageCodes.CreatedSuccessfully;
        public const string UpdatedSuccessfully = MessageCodes.UpdatedSuccessfully;
        public const string DeletedSuccessfully = MessageCodes.DeletedSuccessfully;
        public const string ResourceNotFound = MessageCodes.ResourceNotFound;
        public const string RetrievedSuccessfully = MessageCodes.RetrievedSuccessfully;
        public const string CreationFailed = MessageCodes.CreationFailed;
        public const string UpdateFailed = MessageCodes.UpdateFailed;
        public const string DeletionFailed = MessageCodes.DeletionFailed;
        public const string NoChangesDetected = MessageCodes.NoChangesDetected;
        public const string ConflictDetected = MessageCodes.ConflictDetected;
    }

    /// <summary>All validation message codes.</summary>
    public static class Validation
    {
        public const string ValidationFailed = MessageCodes.ValidationFailed;
        public const string RequiredFieldMissing = MessageCodes.RequiredFieldMissing;
        public const string InvalidFormat = MessageCodes.InvalidFormat;
        public const string ValueOutOfRange = MessageCodes.ValueOutOfRange;
        public const string InvalidEmail = MessageCodes.InvalidEmail;
        public const string InvalidPhoneNumber = MessageCodes.InvalidPhoneNumber;
        public const string PasswordTooWeak = MessageCodes.PasswordTooWeak;
        public const string PasswordsDontMatch = MessageCodes.PasswordsDontMatch;
        public const string InvalidDate = MessageCodes.InvalidDate;
        public const string ValueTooShort = MessageCodes.ValueTooShort;
        public const string ValueTooLong = MessageCodes.ValueTooLong;
        public const string InvalidUrl = MessageCodes.InvalidUrl;
        public const string InvalidFileExtension = MessageCodes.InvalidFileExtension;
        public const string DuplicateValue = MessageCodes.DuplicateValue;
        public const string InvalidCharacters = MessageCodes.InvalidCharacters;
    }

    // Add other category groups as needed...

    #endregion
}