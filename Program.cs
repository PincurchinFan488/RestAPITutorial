
using System.Net.Mime;
using Catalog.Repositories;
using Catalog.Settings;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
var mongoDbsettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
builder.Services.AddSingleton<IMongoClient>(ServiceProvider => 
{
    
    return new MongoClient(mongoDbsettings.ConnectionString);
});
builder.Services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddMongoDb(
        mongoDbsettings.ConnectionString, 
        name: "mongodb", 
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] {"ready"});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = (check) => check.Tags.Contains("ready"),
        ResponseWriter = async(context, report) =>
        {
            var result = JsonSerializer.Serialize(
                new{
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                        duration = entry.Value.Duration.ToString()
                    })
                }
            );

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        }
    });
    app.UseHealthChecks("/health/live", new HealthCheckOptions{
        Predicate = (_) => false
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
