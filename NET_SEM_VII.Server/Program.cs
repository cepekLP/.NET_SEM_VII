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
var ent = new Entity();
ent.SensorId = "test";
ent.Value = 1;
ent.Date = DateTime.Now;
ent.SensorType = "test";
sensorsService.SaveEntity(ent);
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
var mqqttController = new MQTTController();

app.Map("/ws", async context => {
if (context.WebSockets.IsWebSocketRequest)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        //Get data from db that where there before initialization
        string startindData = JsonSerializer.Serialize(sensorsService.GetAllEntities().Result);
        Console.WriteLine("New socket connected sending data stored in db: ");
        Console.WriteLine(startindData);
        if(ws.State == WebSocketState.Open)
        {
            await ws.SendAsync(Encoding.UTF8.GetBytes(startindData),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        mqqttController.ApplicationMessageReceivedAsync += e =>
        {
            //e -> sensorType
            var result = sensorsService.GetLastHundred(e);
            Console.WriteLine("New data incame");
            Console.WriteLine("Sensor id=" + e);
            Console.WriteLine(JsonSerializer.Serialize(result));
            ws.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result)),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            return Task.CompletedTask;
        };
        while (true)
        {

            if (ws.State != WebSocketState.Closed || ws.State != WebSocketState.Open)
            {
                break;
            }
            //Thread.Sleep(500);
        }
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});


await app.RunAsync();
