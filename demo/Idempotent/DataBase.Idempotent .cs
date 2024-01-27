using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CYQ.Data;
using Taurus.Plugin.Idempotent;
namespace DistributedLockTest
{
    /// <summary>
    /// DataBase 幂等性
    /// </summary>
    class DataBaseIdempotentDemo
    {
        static private Idempotent dsLock;
        public static void Start()
        {
            IdempotentConfig.Conn = "server=.;database=mslog;uid=sa;pwd=123456";//由数据库链接决定启用什么链接
            IdempotentConfig.TableName = "taurus_lock";
            dsLock = Idempotent.GetDataBaseLock("taurus_Idempotent");
            for (int i = 0; i < 10000; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(IdempotentThread), i);
            }
        }
        static int ok = 0;
        static int fail = 0;
        static void IdempotentThread(object i)
        {
            string key = "myLock";
            bool isOK = false;
            try
            {
                // 2 秒内只允许进入一个。
                isOK = dsLock.Lock(key, 2.0 / 60);
                if (isOK)
                {
                    Interlocked.Increment(ref ok);
                    Console.WriteLine(ok + " - OK");
                    //Console.WriteLine("数字：" + i + " -- 线程ID：" + Thread.CurrentThread.ManagedThreadId + " 获得锁成功。");
                }
                else
                {
                    Interlocked.Increment(ref fail);
                    if ((fail + ok) % 1000 == 0)
                    {
                        Console.WriteLine(fail + "Fail ----------------------------");
                    }
                    //
                    //Console.WriteLine("数字：" + i + " -- 线程ID：" + Thread.CurrentThread.ManagedThreadId + " 获得锁失败！");
                }
            }
            finally
            {

            }
        }
    }
}
