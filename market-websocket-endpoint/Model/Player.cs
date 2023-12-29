namespace MarketWebsocketEndpoint.Model
{
    public class Player
    {
        public Guid Id { get; set; }
        public double TotalDeliveredEnergy { get; set; }
        public List<EnergyDelivery> EnergyDeliveries { get; set; } = new List<EnergyDelivery>();
    }
}
