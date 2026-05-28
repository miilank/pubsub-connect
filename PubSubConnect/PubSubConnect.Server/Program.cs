using PubSubConnect.Server.Hubs;
using PubSubConnect.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<CupidService>();
builder.Services.AddSingleton<ICupidService>(p => p.GetRequiredService<CupidService>());
builder.Services.AddHostedService(p => p.GetRequiredService<CupidService>());

var app = builder.Build();

app.MapHub<CupidHub>("/kupidon");

app.Run();
