using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Formatters;

public class ConsoleFormatter : IMessageFormatter
{
    private readonly bool _useColors;
    private readonly bool _showTimestamp;

    public ConsoleFormatter(bool useColors = true, bool showTimestamp = true)
    {
        _useColors = useColors;
        _showTimestamp = showTimestamp;
    }

    public string Format(Message message)
    {
        var icon = GetIcon(message.Type);
        var timestamp = _showTimestamp ? $"[{message.Timestamp:HH:mm:ss}] " : "";

        return $"{icon} {message.Title}\n  {message.Description}\n  {timestamp}[{message.Code}]";
    }

    public object FormatAsObject(Message message)
    {
        return new
        {
            Icon = GetIcon(message.Type),
            Color = GetColor(message.Type),
            Message = Format(message),
        };
    }

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
