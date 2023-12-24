namespace market_websocket_endpoint.ModelDto
{
    public class EnergyDeliveryRequest
    {
        public string Id;
        public DateTimeOffset DeliveryStart;
        public DateTimeOffset DeliveryEnd;
        public float Amount;
    }
}
