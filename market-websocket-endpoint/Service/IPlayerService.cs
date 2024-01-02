using MarketWebsocketEndpoint.Model;
using MarketWebsocketEndpoint.ModelDto;

namespace MarketWebsocketEndpoint.Service
{
    public interface IPlayerService
    {
        Task<ProductionDeliveryResponse> DeliverEnergy(ProductionDeliveryRequest delivery, PlayerSession session);
    }
}
