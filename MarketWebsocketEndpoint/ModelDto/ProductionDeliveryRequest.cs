namespace MarketWebsocketEndpoint.ModelDto
{
    public class ProductionDeliveryRequest
    {
        public Guid Id { get; set; }
        public DateTimeOffset DeliveryStart { get; set; }
        public DateTimeOffset DeliveryEnd { get; set; }
        public float Amount { get; set; }
    }
}
