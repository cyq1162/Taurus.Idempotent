using CYQ.Data.Cache;

namespace Taurus.Plugin.Idempotent
{
    internal class MemCacheIdempotent : Idempotent
    {
        private static readonly MemCacheIdempotent _instance = new MemCacheIdempotent();
        private MemCacheIdempotent() { }
        public static MemCacheIdempotent Instance
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
                return IdempotentType.MemCache;
            }
        }


        public override bool Lock(string key)
        {
            return Lock(key, 0);
        }

        public override bool Lock(string key, double keepMinutes)
        {
            key = "I_" + key;
            return DistributedCache.MemCache.SetNXAll(key, "1", keepMinutes);
        }
        public override void Remove(string key)
        {
            DistributedCache.MemCache.RemoveAll(key);
        }
    }
}
