namespace MarketWebsocketEndpoint.ModelDto
{
    public class WebSocketRequestWrapper
    {
        public MessageTypeEnum MessageType { get; set; }
        public string Payload { get; set; } = string.Empty;


    }
}
