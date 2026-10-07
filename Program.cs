using Hubs;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddScoped<ICommandsService, CommandsService>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHub<MainHub>("/Hub");

app.Run();
