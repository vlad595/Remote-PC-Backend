using Hubs;
using Services;
using System.IO.Compression;

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

app.MapGet("/api/download-agent", (HttpContext context) =>
{
    string newAgentId = Guid.NewGuid().ToString();

    context.Response.Headers.Append("X-Agent-Id", newAgentId);
    
    context.Response.Headers.Append("Access-Control-Expose-Headers", "X-Agent-Id");

    string configContent = $"{{\n  \"agentId\": \"{newAgentId}\"\n}}";

    byte[] zipBytes;

    using (var memoryStream = new MemoryStream())
    {
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var configFile = archive.CreateEntry("config.json");
            using (var entryStream = configFile.Open())
            using (var streamWriter = new StreamWriter(entryStream))
            {
                streamWriter.Write(configContent);
            }

            if (File.Exists("TelemetryCollectorService.Agent.exe"))
            {
                var exeFile = archive.CreateEntry("TelemetryCollectorService.Agent.exe");
                using (var exeStream = exeFile.Open())
                using (var fileStream = File.OpenRead("TelemetryCollectorService.Agent.exe"))
                {
                    fileStream.CopyTo(exeStream);
                }
            }
        } 

        zipBytes = memoryStream.ToArray();
    }

    return Results.File(zipBytes, "application/zip", "RemoteMonitorAgent.zip");
});

app.MapHub<MainHub>("/Hub");

app.Run();
