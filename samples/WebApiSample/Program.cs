using System.IO;
using System.Linq;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Attempt to load custom messages from common locations so the sample demonstrates overrides
var candidates = new[]
{
    "messages/custom.json",
    "custom.json",
    "../messages/custom.json",
    "../../messages/custom.json",
};
var searchPaths = candidates
    .Select(p => Path.Combine(builder.Environment.ContentRootPath, p))
    .Concat(candidates);
var found = searchPaths.FirstOrDefault(File.Exists);
if (!string.IsNullOrEmpty(found))
{
    try
    {
        MessageRegistry.Configure(new FileMessageStore(found));
        Console.WriteLine($"Loaded custom messages from: {found}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Failed to load custom messages from {found}: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast")
    .WithOpenApi();

// Simple message endpoint to demonstrate EasyMessages integration
app.MapGet(
        "/message/{code}",
        (string code) =>
        {
            try
            {
                var msg = MessageRegistry.Get(code);
                return Results.Json(msg);
            }
            catch
            {
                return Results.NotFound();
            }
        }
    )
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
