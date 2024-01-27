using CYQ.Data.Cache;
using System.Threading;

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
        public override IdempotentType IdempotentType
        {
            get
            {
                return IdempotentType.MemCache;
            }
        }

        public override bool Lock(string key, double keepMinutes)
        {
            string flag = ProcessID + "," + Thread.CurrentThread.ManagedThreadId + "," + key;
            return DistributedCache.MemCache.SetNXAll(key, flag, keepMinutes);
        }
        public override void Remove(string key)
        {
            DistributedCache.MemCache.RemoveAll(key);
        }
    }
}
