namespace MarketWebsocketEndpoint.Model
{
    public class EnergyDelivery
    {
        public DateTimeOffset DeliveryStart { get; set; }
        public DateTimeOffset DeliveryEnd { get; set; }
        public float Amount { get; set; }
    }
}
