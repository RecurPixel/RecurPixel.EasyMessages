using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages.Facades;

public static class Msg
{
    public static AuthMessages Auth => new();
    public static CrudMessages Crud => new();
    public static ValidationMessages Validation => new();
    public static SystemMessages System => new();
    public static DatabaseMessages Database => new();
    public static FileMessages File => new();

    public static Message Custom(string code)
    {
        return CustomMessages.Get(code);
    }
}