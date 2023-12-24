namespace market_websocket_endpoint.ModelDto
{
    public class EnergyDeliveryRequest
    {
        public string Id { get; set; }
        public DateTimeOffset DeliveryStart { get; set; }
        public DateTimeOffset DeliveryEnd { get; set; }
        public float Amount { get; set; }
    }
}
