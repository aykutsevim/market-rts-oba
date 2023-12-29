using MarketWebsocketEndpoint.Core;
using MarketWebsocketEndpoint.ModelDto;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using MarketWebsocketEndpoint.Model;
using MarketWebsocketEndpoint.Service;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();


if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

var wsOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
};


app.UseWebSockets(wsOptions);

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await WebSocketHandler(webSocket, context);
        }
        else
        {
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        }
    }
    else
    {
        await next();
    }
});



app.Run();

/*
async Task Send(HttpContext context, WebSocket webSocket)
{

}*/


async Task WebSocketHandler(WebSocket webSocket, HttpContext context)
{
    var session = new PlayerSession(Guid.NewGuid(), webSocket);
    // Handle WebSocket connections
    var connectionId = MarketSessionManager.AddSession(session);

    // Your WebSocket handling logic goes here
    var buffer = new byte[1024 * 4];

    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Text)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Console.WriteLine(message);

            // Parse message to EnergyDeliveryRequest
            var energyDeliveryRequest = JsonSerializer.Deserialize<EnergyDeliveryRequest>(message, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (energyDeliveryRequest == null)
            {
                continue;
            }

            // Connect to redis
            var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnectionString");

            if (redisConnectionString == null)
            {
                throw new Exception("Redis connection string is missing");
            }

            using (var redis = ConnectionMultiplexer.Connect(redisConnectionString))
            {
                var service = new PlayerService(redis);

                var energyDeliveryResponse = await service.DeliverEnergy(energyDeliveryRequest, session);

                var response = JsonSerializer.Serialize<EnergyDeliveryResponse>(energyDeliveryResponse);

                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(response)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        else if (result.MessageType == WebSocketMessageType.Close)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
    // Optionally, remove the WebSocket from the manager when the connection is closed
    MarketSessionManager.RemoveSocket(connectionId);
}


/*
var subscriber = redis.GetSubscriber();
subscriber.Subscribe(channel: "MarketPrices", async (channel, message) =>
{
    // Forward the Redis message to connected WebSocket clients
    Console.WriteLine("Subscribed");
    //await MarketWebSocketManager.SendToAllAsync("eventName", message);
});
*/
