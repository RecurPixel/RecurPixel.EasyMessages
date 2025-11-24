using RecurPixel.EasyMessages;
using Xunit;

namespace RecurPixel.EasyMessages.Tests.Unit;

public class MsgFacadeTests : UnitTestBase
{
    [Fact]
    public void Auth_LoginFailed_ShouldReturnCorrectMessage()
    {
        Dispose();
        // Act
        var message = Msg.Auth.LoginFailed();

        // Assert
        Assert.Equal("AUTH_001", message.Code);
        Assert.Equal("Authentication Failed", message.Title);
    }

    [Fact]
    public void Auth_Unauthorized_ShouldReturnCorrectMessage()
    {
        Dispose();
        // Act
        var message = Msg.Auth.Unauthorized();

        // Assert
        Assert.Equal("AUTH_002", message.Code);
        Assert.Equal(403, message.HttpStatusCode);
    }

    [Fact]
    public void Auth_LoginSuccessful_ShouldReturnCorrectMessage()
    {
        Dispose();
        // Act
        var message = Msg.Auth.LoginSuccessful();

        // Assert
        Assert.Equal("AUTH_003", message.Code);
        Assert.Equal(MessageType.Success, message.Type);
    }

    [Fact]
    public void Crud_Created_ShouldSubstituteResource()
    {
        Dispose();
        // Act
        var message = Msg.Crud.Created("User");

        // Assert
        Assert.Equal("CRUD_001", message.Code);
        Assert.Contains("User has been created", message.Description);
    }

    [Fact]
    public void Crud_NotFound_ShouldSubstituteResource()
    {
        Dispose();
        // Act
        var message = Msg.Crud.NotFound("Product");

        // Assert
        Assert.Equal("CRUD_004", message.Code);
        Assert.Contains("Product", message.Description);
        Assert.Equal(404, message.HttpStatusCode);
    }

    [Fact]
    public void Validation_RequiredField_ShouldSubstituteField()
    {
        Dispose();
        // Act
        var message = Msg.Validation.RequiredField("Email");

        // Assert
        Assert.Equal("VAL_002", message.Code);
        Assert.Contains("The field 'Email' is required.", message.Description);
    }

    [Fact]
    public void System_Error_ShouldReturnCorrectMessage()
    {
        Dispose();
        // Act
        var message = Msg.System.Error();

        // Assert
        Assert.Equal("SYS_001", message.Code);
        Assert.Equal(500, message.HttpStatusCode);
    }

    [Fact]
    public void File_InvalidType_ShouldHandleMultipleTypes()
    {
        Dispose();
        // Act
        var message = Msg.File.InvalidType("PDF", "DOCX", "TXT");

        // Assert
        Assert.Equal("FILE_002", message.Code);
        Assert.Contains("PDF", message.Description);
        Assert.Contains("DOCX", message.Description);
        Assert.Contains("TXT", message.Description);
    }

    [Fact]
    public void Custom_ShouldAccessAnyCode()
    {
        Dispose();
        // Act
        var message = Msg.Custom("AUTH_001");

        // Assert
        Assert.Equal("AUTH_001", message.Code);
    }
}
