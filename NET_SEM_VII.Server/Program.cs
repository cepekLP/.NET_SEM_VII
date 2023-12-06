using NET_SEM_VII.Server;
using NET_SEM_VII.Server.Controllers;
using System.Net;
using System.Net.WebSockets;
using System.Text;

//Database db = new Database();
//db.Test();
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
        var message = "Hello World!";
        var bytes = Encoding.UTF8.GetBytes(message);
        var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        while (true)
        {
            if(ws.State == WebSocketState.Open)
            {
                Console.WriteLine("Sending message!");
                await ws.SendAsync(arraySegment,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
            }
            else if(ws.State != WebSocketState.Closed || ws.State != WebSocketState.Open)
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
mqqttController.Init();
await app.RunAsync();
