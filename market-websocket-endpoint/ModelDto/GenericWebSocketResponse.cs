namespace MarketWebsocketEndpoint.ModelDto
{
    public class GenericWebSocketResponse
    {
        public MessageTypeEnum MessageType { get; set; }
        public string Payload { get; set; } = string.Empty;
    }
}
