namespace Taurus.Plugin.Idempotent
{
    /// <summary>
    /// 锁的类型
    /// </summary>
    public enum IdempotentType
    {
        /// <summary>
        /// 基于数据库的分布式锁
        /// </summary>
        DataBase,
        /// <summary>
        /// 基于Redis的分布式锁
        /// </summary>
        Redis,
        /// <summary>
        /// 基于MemCache的分布式锁
        /// </summary>
        MemCache,
        /// <summary>
        /// 基于本机的文件锁，允许其它进程或线程释放。
        /// </summary>
        File
    }
}
