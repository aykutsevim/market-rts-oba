using MarketWebsocketEndpoint.Model;
using MarketWebsocketEndpoint.ModelDto;
using MarketWebsocketEndpoint.Service;
using StackExchange.Redis;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace MarketWebsocketEndpoint.Core
{
    public class WebSocketHandler
    {
        public static string REDIS_CONNECTION_STRING = String.Empty;
        public static Task HandleConsumptionDeliveryRequest(string payload)
        {
            throw new NotImplementedException();
        }

        public static async Task HandleProductionDeliveryRequest(PlayerSession session, string payload) 
        {
            // Parse message to EnergyDeliveryRequest
            var productionDeliveryRequest = JsonSerializer.Deserialize<ProductionDeliveryRequest>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (productionDeliveryRequest == null)
            {
                return;
            }

            // Connect to redis
            var redisConnectionString = REDIS_CONNECTION_STRING;

            if (redisConnectionString == null)
            {
                throw new Exception("Redis connection string is missing");
            }

            using (var redis = ConnectionMultiplexer.Connect(redisConnectionString))
            {
                var service = new PlayerService(redis);

                var energyDeliveryResponse = await service.DeliverEnergy(productionDeliveryRequest, session);

                var responsePayload = JsonSerializer.Serialize<ProductionDeliveryResponse>(energyDeliveryResponse);

                var genericResponse = new GenericWebSocketResponse { MessageType = MessageTypeEnum.ProductionDeliveryResponse, Payload = responsePayload };

                var genericResponseString = JsonSerializer.Serialize<GenericWebSocketResponse>(genericResponse);

                await session.WebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(genericResponseString)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public static async Task Handle(WebSocket webSocket, HttpContext context)
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

                    var genericRequest = JsonSerializer.Deserialize<GenericWebSocketRequest>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                    switch (genericRequest.MessageType)
                    {   
                        case MessageTypeEnum.ConsumptionDeliveryRequest:
                            break;
                        case MessageTypeEnum.ProductionDeliveryRequest:
                            await HandleProductionDeliveryRequest(session, genericRequest.Payload);
                            break;
                        default:
                            break;
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

    }
}
