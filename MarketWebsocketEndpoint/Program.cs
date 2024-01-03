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

WebSocketHandler.REDIS_CONNECTION_STRING = builder.Configuration.GetConnectionString("RedisConnectionString");

app.UseWebSockets(wsOptions);

using (var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnectionString")))
{
    var subscriber = redis.GetSubscriber();
    subscriber.Subscribe(channel: "priceChannel", async (channel, message) =>
    {
        // Forward the Redis message to connected WebSocket clients
        Console.WriteLine("Subscribed");
        await MarketSessionManager.SendToAllAsync("priceChanged", message);
    });
}



app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await WebSocketHandler.Handle(webSocket, context);
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


