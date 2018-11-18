using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Codes
{
    public class RedisClientSingleton
    {
        private static RedisClient _redisClinet;
        private RedisClientSingleton() { }

        private static object _lockObj = new object();
        public static RedisClient GetInstance(IConfigurationRoot config)
        {
            if (_redisClinet == null)
            {
                lock (_lockObj)
                {
                    if (_redisClinet == null)
                    {
                        _redisClinet = new RedisClient(config);
                    }
                }
            }
            return _redisClinet;
        }
    }
}
