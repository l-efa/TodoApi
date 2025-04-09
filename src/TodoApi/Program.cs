using Microsoft.EntityFrameworkCore;
using LoginApi.Models;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.Text.Json;


var configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json")
.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)  // This works now
    .CreateLogger();

Log.Information("päällä");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<LoginContext>(opt =>
    opt.UseInMemoryDatabase("UserList"));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


app.Use(async (context, next) =>
{
    // Log the incoming request method and URL
    Log.Information("Request Method: {Method} | Request Path: {Path}", context.Request.Method, context.Request.Path);

    // Capture the original response body stream
    var originalResponseBodyStream = context.Response.Body;

    // Create a new memory stream to capture the response body
    using (var responseMemoryStream = new MemoryStream())
    {
        // Replace the response body with our memory stream
        context.Response.Body = responseMemoryStream;

        try
        {
            // Call the next middleware (this processes the request and generates the response)
            await next.Invoke();

            // After the response is generated, capture the response body
            responseMemoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseMemoryStream).ReadToEndAsync();

            if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
            {
                Log.Error("Response Status Code: {StatusCode}",
                context.Response.StatusCode);
            }
            else
            {
                // Log the response status code and body (if available)
                Log.Information("Response Status Code: {StatusCode}",
                context.Response.StatusCode);
            }

            // Copy the memory stream back to the original response body so the client gets it
            responseMemoryStream.Seek(0, SeekOrigin.Begin);
            await responseMemoryStream.CopyToAsync(originalResponseBodyStream);
        }
        catch (Exception ex)
        {
            // Log any errors that occur during the request processing
            Log.Error(ex, "An error occurred while processing the request.");

            // Set the response status code to 500 if an exception occurred
            context.Response.StatusCode = 500;
            var errorResponse = new { message = "An error occurred while processing your request." };

            // Write the error response back to the client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
});




app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
