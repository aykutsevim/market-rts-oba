using StackExchange.Redis;

namespace MarketWebsocketEndpoint.ModelDto
{
    public class MarketPriceUpdateResponse
    {
        public string EventName { get; set; }
        public string Message { get; set; }
        public string NewPrice { get; set; }

    }
}
