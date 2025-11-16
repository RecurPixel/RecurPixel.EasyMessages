using RecurPixel.EasyMessages.Core;
using System.Data.SqlClient;

namespace RecurPixel.EasyMessages.Storage;

/// <summary>
/// Loads messages from database
/// </summary>
public class DatabaseMessageStore : IMessageStore
{
    private readonly string _connectionString;

    public DatabaseMessageStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        // await using var connection = new SqlConnection(_connectionString);
        // await connection.OpenAsync();

        var messages = new Dictionary<string, MessageTemplate>();

        // var command = new SqlCommand(
        //     "SELECT Code, Type, Title, Description, HttpStatusCode FROM Messages",
        //     connection
        // );
        // await using var reader = await command.ExecuteReaderAsync();

        // while (await reader.ReadAsync())
        // {
        //     var code = reader.GetString(0);
        //     messages[code] = new MessageTemplate
        //     {
        //         Type = Enum.Parse<MessageType>(reader.GetString(1)),
        //         Title = reader.GetString(2),
        //         Description = reader.GetString(3),
        //         HttpStatusCode = reader.IsDBNull(4) ? null : reader.GetInt32(4),
        //     };
        // }

        return messages;
    }
}


// // Enterprise: Centralized message management
// var store = new DatabaseMessageStore("Server=...;Database=Messages");
// var messages = await store.LoadAsync();