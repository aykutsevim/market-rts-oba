using System.Net.WebSockets;

namespace MarketWebsocketEndpoint.Model
{
    public class PlayerSession
    {
        public Guid Id { get; set; }
        public WebSocket WebSocket { get; set; }
        public Player? Player { get; set; }

        public PlayerSession(Guid id, WebSocket? webSocket)
        {
            Id = id;

            // Check if the WebSocket is null
            if (webSocket == null)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            WebSocket = webSocket;
        }
    }
}
