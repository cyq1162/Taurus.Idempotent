
using CYQ.Data.Cache;
using System.Threading;

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
        public override IdempotentType IdempotentType
        {
            get
            {
                return IdempotentType.Redis;
            }
        }


        public override bool Lock(string key, double keepMinutes)
        {
            string flag = ProcessID + "," + Thread.CurrentThread.ManagedThreadId + "," + key;
            return DistributedCache.Redis.SetNXAll(key, flag, keepMinutes);
        }


        public override void Remove(string key)
        {
            DistributedCache.Redis.RemoveAll(key);
        }
    }
}
