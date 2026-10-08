using Hubs;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); 
    });
});

builder.Services.AddScoped<ICommandsService, CommandsService>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();

var app = builder.Build();

app.UseCors("AllowAll");

app.MapGet("/", () => "Hello World!");

app.MapHub<MainHub>("/Hub");

app.Run();
