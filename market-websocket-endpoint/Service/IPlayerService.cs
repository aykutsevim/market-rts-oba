using MarketWebsocketEndpoint.Model;
using MarketWebsocketEndpoint.ModelDto;

namespace MarketWebsocketEndpoint.Service
{
    public interface IPlayerService
    {
        Task<EnergyDeliveryResponse> DeliverEnergy(EnergyDeliveryRequest delivery, PlayerSession session);
    }
}
