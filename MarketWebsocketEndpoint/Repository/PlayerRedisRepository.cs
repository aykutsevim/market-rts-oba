using MarketWebsocketEndpoint.Model;
using StackExchange.Redis;
using System.Numerics;
using System.Text.Json;

namespace MarketWebsocketEndpoint.Repository
{
    public class PlayerRedisRepository : BaseRedisRepository<Player>
    {
        public PlayerRedisRepository(ConnectionMultiplexer redisConnection) : base(redisConnection)
        {
        }

        protected override Player? Deserialize(string serialized)
        {
            return JsonSerializer.Deserialize<Player>(serialized);
        }

        protected override string GetKey(Player entity)
        {
            return entity.Id.ToString();
        }

        protected override string Serialize(Player entity)
        {
            return JsonSerializer.Serialize(entity);
        }
    }
}
