namespace MarketWebsocketEndpoint.ModelDto
{
    public class WebSocketResponseWrapper
    {
        public MessageTypeEnum MessageType { get; set; }
        public string Payload { get; set; } = string.Empty;
    }
}
