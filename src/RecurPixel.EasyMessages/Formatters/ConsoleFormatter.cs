namespace RecurPixel.EasyMessages.Formatters;

/// <summary>
/// Formats messages for console output.
/// </summary>
public class ConsoleFormatter : MessageFormatterBase
{
    /// <summary>
    /// Indicates whether to use colors in the console output.
    /// </summary>
    private readonly bool _useColors;

    /// <summary>
    /// Indicates whether to show timestamp in the console output.
    /// </summary>
    private readonly bool _showTimestamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleFormatter"/> class.
    /// </summary>
    /// <param name="useColors">boolean value</param>
    /// <param name="showTimestamp">boolean value</param>
    public ConsoleFormatter(bool useColors = true, bool showTimestamp = true)
    {
        _useColors = useColors;
        _showTimestamp = showTimestamp;
    }

    /// <summary>
    /// Formats the message for console output.
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Formated Message String</returns>
    protected override string FormatCore(Message message)
    {
        var icon = GetIcon(message.Type);
        var timestamp = _showTimestamp ? $"[{message.Timestamp:HH:mm:ss}] " : "";

        return $"{icon} {message.Title}\n  {message.Description}\n  {timestamp}[{message.Code}]\n {message.ToJson()}";
    }

    /// <summary>
    /// Formats the message as an object for console output.
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <returns>Formated Message Object</returns>
    public override object FormatAsObject(Message message)
    {
        return new
        {
            Icon = GetIcon(message.Type),
            Color = GetColor(message.Type),
            Message = Format(message),
        };
    }

    /// <summary>
    /// Gets the icon for the message type.
    /// </summary>
    /// <param name="type">Message Type</param>
    /// <returns>Icon</returns>
    private static string GetIcon(MessageType type) =>
        type switch
        {
            MessageType.Success => "✓",
            MessageType.Info => "ℹ",
            MessageType.Warning => "⚠",
            MessageType.Error => "✗",
            MessageType.Critical => "☠",
            _ => "•",
        };

    /// <summary>
    /// Gets the console color for the message type.
    /// </summary>
    /// <param name="type">Message Type</param>
    /// <returns>ConsoleColor</returns>
    private static ConsoleColor GetColor(MessageType type) =>
        type switch
        {
            MessageType.Success => ConsoleColor.Green,
            MessageType.Info => ConsoleColor.Cyan,
            MessageType.Warning => ConsoleColor.Yellow,
            MessageType.Error => ConsoleColor.Red,
            MessageType.Critical => ConsoleColor.DarkRed,
            _ => ConsoleColor.White,
        };

    /// <summary>
    /// Writes the formatted message to the console.
    /// </summary>
    /// <param name="message">Message Object</param>
    public void WriteToConsole(Message message)
    {
        if (!_useColors)
        {
            Console.WriteLine(Format(message));
            return;
        }

        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = GetColor(message.Type);
        Console.WriteLine(Format(message));
        Console.ForegroundColor = originalColor;
    }
}
