using MarketWebsocketEndpoint.Model;
using MarketWebsocketEndpoint.ModelDto;
using MarketWebsocketEndpoint.Repository;
using StackExchange.Redis;
using System.Text.Json;

namespace MarketWebsocketEndpoint.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly ConnectionMultiplexer _redisConnection;

        public PlayerService(ConnectionMultiplexer redisConnection) 
        {
            _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        }

        public async Task<EnergyDeliveryResponse> DeliverEnergy(EnergyDeliveryRequest delivery, PlayerSession session)
        {
            if (session.Player == null)
            {
                session.Player = new Player()
                {
                    Id = delivery.Id,
                    TotalDeliveredEnergy = delivery.Amount
                };
            }
            else
            {
                session.Player.TotalDeliveredEnergy += delivery.Amount;
            }

            session.Player.EnergyDeliveries.Add(new EnergyDelivery()
            {
                DeliveryStart = delivery.DeliveryStart,
                DeliveryEnd = delivery.DeliveryEnd,
                Amount = delivery.Amount
            });

            var playerRepository = new PlayerRedisRepository(_redisConnection);

            playerRepository.Save(session.Player);

            Console.WriteLine($"Total delivered for {session.Player.Id}: {session.Player.TotalDeliveredEnergy}");

            var energyDeliveryResponse = new EnergyDeliveryResponse
            {
                TotalDeliveredEnergy = session.Player.TotalDeliveredEnergy,
                TotalConsumedEnergy = 0.0f
            };

            return energyDeliveryResponse;
        }
    }
}
