var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/api/hello", () => "Hello World!");

app.Run();
