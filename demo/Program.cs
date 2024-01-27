using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DistributedLockTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine("1、File.Idempotent");
            Console.WriteLine("2、DataBase.Idempotent");
            Console.WriteLine("3、Redis.Idempotent");
            Console.WriteLine("4、MemCache.Idempotent");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("Input num to choose : ");
            string key = Console.ReadLine();
            while (true)
            {
                switch (key)
                {
                    case "2":
                        DataBaseIdempotentDemo.Start();
                        break;
                    case "3":
                        RedisIdempotentDemo.Start();
                        break;
                    case "4":
                        MemCacheIdempotentDemo.Start();
                        break;
                    default:
                        FileIdempotentDemo.Start();
                        break;
                }
                key = Console.ReadLine();
            }

        }
    }
}
