using MarketWebsocketEndpoint.Model;
using MarketWebsocketEndpoint.ModelDto;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarketWebsocketEndpoint.Core
{
    public class MarketSessionManager
    {
        public const string MARKET_ALPHA_PRICE_CHANGED = "MARKET_ALPHA_PRICE_CHANGED";
        public const string MARKET_BETA_PRICE_CHANGED = "MARKET_BETA_PRICE_CHANGED";

        private static ConcurrentDictionary<string, PlayerSession> _sessions = new ConcurrentDictionary<string, PlayerSession>();

        public static string AddSession(PlayerSession session)
        {
            var connectionId = Guid.NewGuid().ToString();
            _sessions.TryAdd(connectionId, session);
            return connectionId;
        }

        public static async Task RemoveSocket(string connectionId)
        {
            if (_sessions.TryRemove(connectionId, out var session))
            {
                await session.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }
        }

        public static async Task SendToAllAsync(string eventName, string message)
        {
            // Deserialize MarketPriceUpdatedEvent
            var marketPriceUpdatedEvent = JsonSerializer.Deserialize<MarketPriceUpdatedEvent>(message, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (marketPriceUpdatedEvent == null)
            {
                return;
            }

            // Convert MarketPriceUpdatedEvent to MarketPriceUpdateResponse
            var marketPriceUpdateResponse = new MarketPriceUpdateResponse
            {
                MarketName = marketPriceUpdatedEvent.MarketName,
                BuyPrice = marketPriceUpdatedEvent.BuyPrice,
                SellPrice = marketPriceUpdatedEvent.SellPrice,
            };

            // Serialize MarketPriceUpdateResponse
            var responsePayload = JsonSerializer.Serialize(marketPriceUpdateResponse);

            // Wrap MarketPriceUpdateResponse in WebSocketResponseWrapper
            var responseWrapper = new WebSocketResponseWrapper { MessageType = MessageTypeEnum.MarketPriceUpdateResponse, Payload = responsePayload };

            // Serialize WebSocketResponseWrapper
            responsePayload = JsonSerializer.Serialize(responseWrapper);

            // Convert WebSocketResponseWrapper to byte array
            var bytes = Encoding.UTF8.GetBytes(responsePayload);

            // Send MarketPriceUpdateResponse to all connected clients
            foreach (var session in _sessions)
            {
                if (session.Value.WebSocket.State == WebSocketState.Open)
                {
                    await session.Value.WebSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length),
                                                 WebSocketMessageType.Text,
                                                 true,
                                                 CancellationToken.None);
                }
            }
        }
    }
}
