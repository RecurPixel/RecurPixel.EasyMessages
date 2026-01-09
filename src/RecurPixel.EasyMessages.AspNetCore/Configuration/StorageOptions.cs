namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Configuration options for custom message storage
/// </summary>
public class StorageOptions
{
    /// <summary>
    /// Path to a single custom messages JSON file
    /// </summary>
    /// <remarks>
    /// Load custom messages from a JSON file to override built-in messages or add new ones.
    ///
    /// File format:
    /// {
    ///   "MESSAGE_CODE": {
    ///     "type": "Success|Error|Warning|Info",
    ///     "title": "Message title",
    ///     "description": "Message description with {parameters}",
    ///     "httpStatusCode": 200
    ///   }
    /// }
    ///
    /// Priority: Custom messages override built-in messages with the same code.
    /// </remarks>
    /// <example>
    /// "messages/custom.json"
    /// </example>
    public string? CustomMessagesPath { get; set; }

    /// <summary>
    /// Paths to multiple custom message files
    /// </summary>
    /// <remarks>
    /// Load messages from multiple JSON files. Files are loaded in order,
    /// with later files overriding earlier ones for duplicate message codes.
    ///
    /// Loading order (last wins):
    /// 1. Built-in messages (lowest priority)
    /// 2. CustomMessagesPath file
    /// 3. CustomStorePaths files (in array order)
    /// 4. Programmatically registered stores (highest priority)
    ///
    /// Use case: Organize messages by domain or feature:
    /// - messages/auth.json
    /// - messages/payment.json
    /// - messages/validation.json
    /// </remarks>
    /// <example>
    /// new List&lt;string&gt; { "messages/auth.json", "messages/payment.json" }
    /// </example>
    public List<string>? CustomStorePaths { get; set; }
}
