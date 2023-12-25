using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace market_websocket_endpoint.Core
{
    public class MarketWebSocketManager
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public static string AddSocket(WebSocket socket)
        {
            var connectionId = Guid.NewGuid().ToString();
            _sockets.TryAdd(connectionId, socket);
            return connectionId;
        }

        public static async Task RemoveSocket(string connectionId)
        {
            if (_sockets.TryRemove(connectionId, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }
        }

        public static async Task SendToAllAsync(string eventName, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            foreach (var socket in _sockets)
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    await socket.Value.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length),
                                                 WebSocketMessageType.Text,
                                                 true,
                                                 CancellationToken.None);
                }
            }
        }
    }
}
