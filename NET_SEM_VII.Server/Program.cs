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
            List<Entity2> result3 = new List<Entity2>();
            String[] SensorTypes = {"WheelSpeed","WheelTemperature","RideHeight","DamperPosition"};
            String SensorType = e.Split(",")[0];
            String SensorId = e.Split(",")[1];
            //var result = sensorsService.GetLastHundred(SensorType);
            foreach (var SensorT in SensorTypes)
            {
                for (int i = 0; i < 4; i++)
                {
                    var result2 = sensorsService.GetLast100EntitiesByTypeAndID(SensorT, SensorId);

                    Entity2 entity2 = new Entity2();
                    entity2.SensorId = i;
                    entity2.SensorType = SensorT;
                    entity2.CurrentValue = result2.Result[0].Value;
                    entity2.AverageValue = 0;
                    for (int j = 0; j < result2.Result.Count; j++)
                    {
                        entity2.AverageValue += result2.Result[j].Value;
                    }
                    entity2.AverageValue /= result2.Result.Count;
                    result3.Add(entity2);
                }
            }
            Console.WriteLine("New data incame");
            Console.WriteLine("Sensor type=" + SensorType+ " Sensor Id=" +SensorId);
            //Console.WriteLine(JsonSerializer.Serialize(result));
            Console.WriteLine(JsonSerializer.Serialize(result3));
            ws.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result3)),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            //ws.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result)),
            //        WebSocketMessageType.Text,
            //        true,
            //        CancellationToken.None);
            return Task.CompletedTask;
        };
        while (true)
        {

            if (ws.State != WebSocketState.Open)
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


await app.RunAsync();
