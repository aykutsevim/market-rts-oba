using market_websocket_endpoint.ModelDto;
using StackExchange.Redis;

namespace market_websocket_endpoint.Service
{
    public class PlayerService
    {
        private readonly ConnectionMultiplexer _redisConnection;

        public PlayerService(ConnectionMultiplexer redisConnection) 
        {
            _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        }

        public void DeliverEnergy(EnergyDeliveryRequest delivery)
        {

        }
    }
}
