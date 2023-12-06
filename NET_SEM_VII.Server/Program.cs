using MongoDB.Bson.IO;
using NET_SEM_VII.Server;
using NET_SEM_VII.Server.Controllers;
using NET_SEM_VII.Server.db;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

SensorsService sensorsService = new SensorsService();
var json = JsonSerializer.Serialize(sensorsService.GetAllEntities().Result);
Console.WriteLine(json);
//Thread.Sleep(1000);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Console.WriteLine(args);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*builder.WebHost.UseUrls("https://localhost:6969");*/
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseWebSockets();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Map("/ws", async context => {
if (context.WebSockets.IsWebSocketRequest)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        //Get data from db that where there before initialization
        string startindData = JsonSerializer.Serialize(sensorsService.GetAllEntities().Result);
        if(ws.State == WebSocketState.Open)
        {
            await ws.SendAsync(Encoding.UTF8.GetBytes(json),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        while (true)
        {
            if (ws.State != WebSocketState.Closed || ws.State != WebSocketState.Open)
            {
                break;
            }
            Thread.Sleep(500);
        }
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});
var mqqttController = new MQTTController();
await app.RunAsync();
