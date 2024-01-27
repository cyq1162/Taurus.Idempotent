using CYQ.Data;
using CYQ.Data.Cache;
using CYQ.Data.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Taurus.Plugin.Idempotent
{

    /// <summary>
    /// 分布式锁
    /// </summary>
    public abstract partial class Idempotent
    {
        #region 对外实例
        /// <summary>
        /// 分布式锁实例【根据配置顺序取值：DataBase => Redis => MemCache => File】
        /// </summary>
        public static Idempotent Instance
        {
            get
            {
                if (!string.IsNullOrEmpty(IdempotentConfig.Conn))
                {
                    return DataBase;
                }
                if (!string.IsNullOrEmpty(IdempotentConfig.RedisServers))
                {
                    return Redis;
                }
                if (!string.IsNullOrEmpty(IdempotentConfig.MemCacheServers))
                {
                    return MemCache;
                }
                return File;

            }
        }

        /// <summary>
        /// Redis 分布式锁实例
        /// </summary>
        public static Idempotent Redis
        {
            get
            {
                return RedisIdempotent.Instance;
            }
        }

        /// <summary>
        /// MemCach 分布式锁实例
        /// </summary>
        public static Idempotent MemCache
        {
            get
            {
                return MemCacheIdempotent.Instance;
            }
        }

        /// <summary>
        /// Local 单机内 文件锁【可跨进程或线程释放】
        /// </summary>
        public static Idempotent File
        {
            get
            {
                return FileIdempotent.Instance;
            }
        }

        /// <summary>
        /// 数据库 分布式锁默认实例
        /// </summary>
        public static Idempotent DataBase
        {
            get
            {
                return new DataBaseIdempotent(IdempotentConfig.TableName, IdempotentConfig.Conn);
            }
        }
        /// <summary>
        /// 自定义数据库锁实例。
        /// </summary>
        /// <param name="tableName">自定义表名</param>
        /// <returns></returns>
        public static Idempotent GetDataBaseLock(string tableName)
        {
            return new DataBaseIdempotent(tableName, IdempotentConfig.Conn);
        }
        /// <summary>
        /// 自定义数据库锁实例。
        /// </summary>
        /// <param name="tableName">自定义表名</param>
        /// <param name="conn">自定义数据库链接</param>
        /// <returns></returns>
        public static Idempotent GetDataBaseLock(string tableName, string conn)
        {
            return new DataBaseIdempotent(tableName, conn);
        }
        #endregion

        private static int _ProcessID;
        /// <summary>
        /// 当前进程ID
        /// </summary>
        protected static int ProcessID
        {
            get
            {
                if (_ProcessID == 0)
                {
                    _ProcessID = Process.GetCurrentProcess().Id;
                }
                return _ProcessID;
            }
        }

        /// <summary>
        /// 锁类型
        /// </summary>
        public abstract IdempotentType LockType { get; }

        /// <summary>
        /// 幂等性
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public abstract bool Lock(string key);
        /// <summary>
        /// 幂等性
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keepMinutes">数据保留时间，单位分钟，0 则永久。</param>
        /// <returns></returns>
        public abstract bool Lock(string key, double keepMinutes);

        /// <summary>
        /// 手工移除锁
        /// </summary>
        /// <param name="key">key</param>
        public abstract void Remove(string key);
    }
}
