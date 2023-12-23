using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

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
            await Send(context, webSocket);
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

async Task Send(HttpContext context, WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];

    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Text)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Message received: {message}");

            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Response {DateTime.UtcNow:f}")), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else if (result.MessageType == WebSocketMessageType.Close)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}
