using Microsoft.EntityFrameworkCore;
using LoginApi.Models;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using LoginApi.Azure;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Core;
using Microsoft.Data.SqlClient;
using System.IO;


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

/*
builder.Services.Configure<AzureAdOptions>(
    builder.Configuration.GetSection("AzureAd"));
*/

/*
builder.Services.AddDbContext<LoginContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var azureAd = sp.GetRequiredService<IOptions<AzureAdOptions>>().Value;
    var connectionString = config.GetConnectionString("TodoDatabaseConn");

    var credential = new ClientSecretCredential(
        azureAd.TenantId,
        azureAd.ClientId,
        azureAd.ClientSecret);

    //var token = credential.GetToken(
    //new TokenRequestContext(new[] { "https://database.windows.net/.default" }));

    var conn = new SqlConnection(connectionString)
    {
        //AccessToken = token.Token
    };

    options.UseSqlServer(conn);
});*/

builder.Services.AddDbContext<LoginContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDatabaseConn"))
);

/*builder.Services.AddDbContext<LoginContext>(opt =>
    opt.UseInMemoryDatabase("UserList"));*/
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


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
