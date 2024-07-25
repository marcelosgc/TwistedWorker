using Serilog;
using TwistedCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ITwistedService, TwistedService>();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


var app = builder.Build();

//Use swagger at your will 
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.MapGet("/list-log-files", async (ITwistedService twistedService) =>
    {
        var result = await twistedService.GetLogFiles();
        return result.Match(files => files, error => throw error);
    })
    .WithOpenApi();
app.Run();