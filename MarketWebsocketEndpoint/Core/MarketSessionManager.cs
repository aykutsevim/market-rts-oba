using MarketWebsocketEndpoint.Model;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MarketWebsocketEndpoint.Core
{
    public class MarketSessionManager
    {
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
            var bytes = Encoding.UTF8.GetBytes(message);

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
