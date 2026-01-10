using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;
using WebApiSample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "EasyMessages Web API Sample",
        Version = "v1",
        Description = "A comprehensive sample demonstrating RecurPixel.EasyMessages in ASP.NET Core Web APIs",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "RecurPixel",
            Url = new Uri("https://github.com/RecurPixel/RecurPixel.EasyMessages")
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Required for EasyMessages interceptors (correlation ID, metadata enrichment)
builder.Services.AddHttpContextAccessor();

// Register EasyMessages with AspNetCore extensions
// Uses configuration from appsettings.json
builder.Services.AddEasyMessages(builder.Configuration);

// Register application services
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyMessages API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () =>
{
    return Msg.System.OperationCompleted()
        .WithData(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        })
        .ToApiResponse();
}).WithName("HealthCheck");

// Message catalog endpoint - browse all available message codes
app.MapGet("/api/messages", () =>
{
    var allCodes = new[]
    {
        "AUTH_001", "AUTH_002", "AUTH_003", "AUTH_004", "AUTH_005",
        "CRUD_001", "CRUD_002", "CRUD_003", "CRUD_004", "CRUD_005",
        "VAL_001", "VAL_002", "VAL_003", "VAL_004", "VAL_005",
        "SYS_001", "SYS_002", "SYS_003", "SYS_004", "SYS_005",
        "DB_001", "DB_002", "DB_003", "DB_004", "DB_005"
    };

    var messages = allCodes.Select(code =>
    {
        try
        {
            var msg = MessageRegistry.Get(code);
            return new { code, title = msg.Title, type = msg.Type.ToString() };
        }
        catch
        {
            return null;
        }
    }).Where(m => m != null).ToList();

    return Msg.Crud.Retrieved("Messages")
        .WithData(new { messages, count = messages.Count })
        .ToApiResponse();
}).WithName("GetMessageCatalog");

Console.WriteLine(@"
═══════════════════════════════════════════════════════════════
   EasyMessages Web API Sample
═══════════════════════════════════════════════════════════════

   Swagger UI:  http://localhost:5200 (or configured port)

   Quick Start Endpoints:
   • GET  /health                    - Health check
   • GET  /api/messages               - Browse all message codes
   • POST /api/auth/login             - Login example
   • GET  /api/users                  - List users
   • POST /api/users                  - Create user

   Sample Login:
   {
     ""username"": ""admin"",
     ""password"": ""password123""
   }

═══════════════════════════════════════════════════════════════
");

app.Run();
