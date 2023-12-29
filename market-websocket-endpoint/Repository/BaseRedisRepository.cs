using StackExchange.Redis;

namespace MarketWebsocketEndpoint.Repository
{
    public abstract class BaseRedisRepository<T>
    {
        private readonly ConnectionMultiplexer _redisConnection;

        protected BaseRedisRepository(ConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        }

        protected abstract string GetKey(T entity);
        protected abstract T Deserialize(string serialized);
        protected abstract string Serialize(T entity);

        public void Save(T entity)
        {
            IDatabase redisDb = _redisConnection.GetDatabase();
            string key = GetKey(entity);
            string serializedEntity = Serialize(entity);

            redisDb.StringSet(key, serializedEntity);
        }

        public T GetById(Guid id)
        {
            IDatabase redisDb = _redisConnection.GetDatabase();
            string key = id.ToString();

            string serializedEntity = redisDb.StringGet(key);
            if (serializedEntity == null)
            {
                return default; // Entity not found
            }

            return Deserialize(serializedEntity);
        }
    }
}
