using RecurPixel.EasyMessages.Exceptions;

namespace RecurPixel.EasyMessages.Formatters;

/// <summary>
/// Registry for message formatters
/// </summary>
public static class FormatterRegistry
{
    /// <summary>
    /// Factories for creating formatter instances
    /// </summary>
    private static readonly Dictionary<string, Func<IMessageFormatter>> _factories = new();

    /// <summary>
    /// Singleton instances of formatters
    /// </summary>
    private static readonly Dictionary<string, IMessageFormatter> _instances = new();

    /// <summary>
    /// Lock object for thread safety
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    /// Static constructor to register built-in formatters
    /// </summary>
    static FormatterRegistry()
    {
        // Register built-in formatters
        Register("json", () => new JsonFormatter());
        Register("xml", () => new XmlFormatter());
        Register("text", () => new PlainTextFormatter());
        Register("console", () => new ConsoleFormatter());
        Register("log", () => new LogFormatter());
    }

    /// <summary>
    /// Register a formatter with a factory function
    /// </summary>
    public static void Register(string name, Func<IMessageFormatter> factory)
    {
        lock (_lock)
        {
            _factories[name.ToLowerInvariant()] = factory;
        }
    }

    /// <summary>
    /// Register a formatter type (requires parameterless constructor)
    /// </summary>
    public static void Register<TFormatter>(string? name = null)
        where TFormatter : IMessageFormatter, new()
    {
        var key = name ?? typeof(TFormatter).Name.Replace("Formatter", "").ToLowerInvariant();
        Register(key, () => new TFormatter());
    }

    /// <summary>
    /// Register a singleton formatter instance
    /// </summary>
    public static void RegisterSingleton(string name, IMessageFormatter instance)
    {
        lock (_lock)
        {
            var key = name.ToLowerInvariant();
            _instances[key] = instance;
            _factories[key] = () => instance; // Factory returns same instance
        }
    }

    /// <summary>
    /// Get formatter by name (creates new instance each time unless singleton)
    /// </summary>
    public static IMessageFormatter Get(string name)
    {
        lock (_lock)
        {
            var key = name.ToLowerInvariant();

            // Return singleton if registered
            if (_instances.TryGetValue(key, out var instance))
            {
                return instance;
            }

            // Create from factory
            if (_factories.TryGetValue(key, out var factory))
            {
                return factory();
            }

            throw new FormatterNotFoundException(
                $"Formatter '{name}' not found. Available: {string.Join(", ", _factories.Keys)}"
            );
        }
    }

    /// <summary>
    /// Get formatter by type (creates new instance)
    /// </summary>
    public static TFormatter Get<TFormatter>()
        where TFormatter : IMessageFormatter, new()
    {
        return new TFormatter();
    }

    /// <summary>
    /// Check if formatter type is registered
    /// </summary>
    public static bool IsRegistered(string name)
    {
        lock (_lock)
        {
            return _factories.ContainsKey(name.ToLowerInvariant());
        }
    }

    /// <summary>
    /// Get all registered formatter names
    /// </summary>
    public static IEnumerable<string> GetRegisteredNames()
    {
        lock (_lock)
        {
            return _factories.Keys.ToList();
        }
    }

    /// <summary>
    /// Clear all custom formatters (keeps built-ins)
    /// </summary>
    public static void ClearCustom()
    {
        lock (_lock)
        {
            var builtInNames = new[] { "json", "xml", "text", "console", "log" };
            var customKeys = _factories.Keys.Except(builtInNames).ToList();

            foreach (var key in customKeys)
            {
                var type = _factories[key];
                _factories.Remove(key);
                _instances.Remove(key);
            }
        }
    }
}
