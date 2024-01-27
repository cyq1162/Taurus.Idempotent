
using CYQ.Data.Cache;

namespace Taurus.Plugin.Idempotent
{
    internal class RedisIdempotent : Idempotent
    {
        private static readonly RedisIdempotent _instance = new RedisIdempotent();
        private RedisIdempotent() { }
        public static RedisIdempotent Instance
        {
            get
            {
                return _instance;
            }
        }
        public override IdempotentType LockType
        {
            get
            {
                return IdempotentType.Redis;
            }
        }


        public override bool Lock(string key)
        {
            return Lock(key, 0);
        }

        public override bool Lock(string key, double keepMinutes)
        {
            key = "I_" + key;
            return DistributedCache.Redis.SetNXAll(key, "1", keepMinutes);
        }


        public override void Remove(string key)
        {
            DistributedCache.Redis.RemoveAll(key);
        }
    }
}
