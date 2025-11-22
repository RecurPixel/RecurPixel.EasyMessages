using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Validation messages.
    /// </summary>
    public static class Validation
    {
        /// <summary>Returns VAL_001: Please check your input and try again.</summary>
        public static Message Failed() => MessageRegistry.Get(MessageCodes.Validation.ValidationFailed);

        /// <summary>Returns VAL_002: The field '{field}' is required.</summary>
        public static Message RequiredField(string? field = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.RequiredFieldMissing);

            return field != null ? message.WithParams(new { field }) : message;
        }

        /// <summary>Returns VAL_003: The field '{field}' has an invalid format.</summary>
        public static Message InvalidFormat(string? field = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.InvalidFormat);

            return field != null ? message.WithParams(new { field }) : message;
        }

        /// <summary>Returns VAL_004: The field '{field}' must be between {min} and {max}.</summary>
        public static Message OutOfRange(string? field = null, int? min = null, int? max = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.ValueOutOfRange);

            return message.WithParamsIfProvided(new { field, min, max });
        }

        /// <summary>Returns VAL_005: Please provide a valid email address.</summary>
        public static Message InvalidEmail() => MessageRegistry.Get(MessageCodes.Validation.InvalidEmail);

        /// <summary>Returns VAL_006: Please provide a valid phone number.</summary>
        public static Message InvalidPhoneNumber() => MessageRegistry.Get(MessageCodes.Validation.InvalidPhoneNumber);

        /// <summary>Returns VAL_007: Password must meet security requirements.</summary>
        public static Message WeakPassword() => MessageRegistry.Get(MessageCodes.Validation.PasswordTooWeak);

        /// <summary>Returns VAL_008: Password and confirmation password must match.</summary>
        public static Message PasswordMismatch() => MessageRegistry.Get(MessageCodes.Validation.PasswordsDontMatch);

        /// <summary>Returns VAL_009: The field '{field}' contains an invalid date.</summary>
        public static Message InvalidDate(string? field = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.InvalidDate);

            return field != null ? message.WithParams(new { field }) : message;
        }
        

        /// <summary>Returns VAL_010: The field '{field}' must be at least {minLength} characters long.</summary>
        public static Message TooShort(string? field = null, int? minLength = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.ValueTooShort);

            return message.WithParamsIfProvided(new { field, minLength });
        }

        /// <summary>Returns VAL_011: The field '{field}' must not exceed {maxLength} characters.</summary>
        public static Message TooLong(string? field = null, int? maxLength = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.ValueTooLong);

            return message.WithParamsIfProvided(new { field, maxLength });
        }

        /// <summary>Returns VAL_012: Please provide a valid URL.</summary>
        public static Message InvalidUrl() => MessageRegistry.Get(MessageCodes.Validation.InvalidUrl);

        /// <summary>Returns VAL_013: The field '{field}' must be a valid {type}.</summary>
        public static Message InvalidFileExtension(string? field = null, string? type = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.InvalidFileExtension);

            return message.WithParamsIfProvided(new { field, type });
        }

        /// <summary>Returns VAL_014: The value '{value}' already exists.</summary>
        public static Message Duplicate(string? field = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.DuplicateValue);

            return field != null ? message.WithParams(new { field }) : message;
        }

        /// <summary>Returns VAL_015: The field '{field}' contains invalid characters.</summary>
        public static Message InvalidCharacters(string? field = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Validation.InvalidCharacters);

            return field != null ? message.WithParams(new { field }) : message;
        }
    }
}
