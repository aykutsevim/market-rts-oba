using MarketWebsocketEndpoint.ModelDto;

namespace MarketWebsocketEndpoint.Model
{
    public class MarketPriceUpdatedEvent
    {
        public EventTypeEnum EventType { get; set; }
        public string MarketName { get; set; } = String.Empty;
        public Decimal BuyPrice { get; set; }
        public Decimal SellPrice { get; set; }
    }
}
