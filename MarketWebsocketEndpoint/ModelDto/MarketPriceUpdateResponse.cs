using StackExchange.Redis;

namespace MarketWebsocketEndpoint.ModelDto
{
    public class MarketPriceUpdateResponse
    {
        public string MarketName { get; set; } = String.Empty;
        public Decimal BuyPrice { get; set; }
        public Decimal SellPrice { get; set; }

    }
}
