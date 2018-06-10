using System;
using StackExchange.Redis;

namespace Redis
{
    public class RedisHelper
    {
        public IDatabase Database {get; set;}
        public RedisHelper()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
            Database = redis.GetDatabase();
        }          
    }
}
