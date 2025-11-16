using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;


namespace RecurPixel.EasyMessages.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var authFailed = Msg.Auth.LoginFailed();
        Assert.IsType<Message>(authFailed);

    }

    [Fact]
    public void Test2()
    {
        var authFailed = Msg.Auth.LoginFailed();
        Assert.Equal("hello", authFailed.Code);

    }

    [Fact]
    public void Test3()
    {
        var authFailed = Msg.Auth.LoginFailed();
        Assert.Equal("hello", authFailed.Title);

    }

    [Fact]
    public void Test4()
    {
        var authFailed = Msg.Auth.LoginFailed();
        Assert.Equal("hello", authFailed.Description);

    }

    [Fact]
    public void Test5()
    {
        var authFailed = Msg.Auth.LoginFailed();
        Assert.Equal(2000, authFailed.HttpStatusCode);

    }
}