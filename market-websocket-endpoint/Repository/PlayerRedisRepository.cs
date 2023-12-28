using market_websocket_endpoint.Model;
using StackExchange.Redis;
using System.Numerics;
using System.Text.Json;

namespace market_websocket_endpoint.Repository
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
            return entity.ID.ToString();
        }

        protected override string Serialize(Player entity)
        {
            return JsonSerializer.Serialize(entity);
        }
    }
}
