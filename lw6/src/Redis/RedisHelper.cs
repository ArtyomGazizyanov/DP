using System;
using StackExchange.Redis;

namespace Redis
{
    public class RedisHelper
    {   
        public static readonly RedisHelper Instance = new RedisHelper();

		public IDatabase Database { get; private set; }

		private RedisHelper()
		{
			SetDatabase(-1);
		}

        public void SetDatabase(int db = -1) => Database = ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(db);

        public int CalculateDatabase(string data)
		{
			int digitCount = 0;
			foreach (char ch in data)
			{
				if (Char.IsDigit(ch))
				{
					++digitCount;
				}
			}

			return digitCount % 15;
		}
	}
}
