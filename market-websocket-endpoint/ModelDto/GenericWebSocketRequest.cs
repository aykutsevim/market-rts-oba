namespace MarketWebsocketEndpoint.ModelDto
{
    public class GenericWebSocketRequest
    {
        public MessageTypeEnum MessageType { get; set; }
        public string Payload { get; set; } = string.Empty;


    }
}
