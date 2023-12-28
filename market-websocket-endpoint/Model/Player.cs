namespace market_websocket_endpoint.Model
{
    public class Player
    {
        public Guid ID { get; set; }
        public double TotalDelivered { get; set; }
        public List<EnergyDelivery> EnergyDeliveries { get; set; } = new List<EnergyDelivery>();
    }
}
