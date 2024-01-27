using CYQ.Data;
using CYQ.Data.Cache;
using CYQ.Data.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;



namespace Taurus.Plugin.Idempotent
{
    internal partial class FileIdempotent : Idempotent
    {
        private static readonly FileIdempotent _instance = new FileIdempotent();
        string folder = string.Empty;
        private FileIdempotent()
        {
            string path = IdempotentConfig.Path;
            if (!path.Contains(":") && !path.StartsWith("/tmp"))
            {
                //自定义路径
                path = AppConfig.WebRootPath + path.TrimStart('/', '\\');
            }
            folder = path.TrimEnd('/', '\\') + "/TaurusFileIdempotent/";
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
        }
        public static FileIdempotent Instance
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
                return IdempotentType.File;
            }
        }

    }

    internal partial class FileIdempotent
    {

        public override void Remove(string key)
        {
            try
            {
                string path = folder + key + ".lock";
                System.IO.File.Delete(path);
            }
            catch
            {
               
            }
        }
        //private static readonly object lockObj = new object();
        private bool IsLockOK(string key)
        {
            string path = folder + key + ".lock";
            try
            {
                if (System.IO.File.Exists(path))
                {
                    return false;
                }

                System.IO.File.Create(path).Close();
                return true;
            }
            catch (Exception err)
            {

            }
            return false;
        }
    }

    /// <summary>
    /// 处理文件锁【幂等性】
    /// </summary>
    internal partial class FileIdempotent
    {
        public override bool Lock(string key, double keepMinutes)
        {
            if (keepMinutes > 0)
            {
                string path = folder + key + ".lock";
                var mutex = GetMutex(path);
                try
                {
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Exists && fileInfo.LastWriteTime.AddMinutes(keepMinutes) < DateTime.Now)
                    {
                        System.IO.File.Delete(path);
                        return IsLockOK(key);
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }

            }
            return IsLockOK(key);
        }


        private static Mutex GetMutex(string fileName)
        {
            string key = "IO" + fileName.GetHashCode();
            var mutex = new Mutex(false, key);
            try
            {
                mutex.WaitOne();
            }
            catch (AbandonedMutexException ex)
            {
                //其它进程直接关闭，未释放即退出时【锁未对外开放，因此不存在重入锁问题，释放1次即可】。
                mutex.ReleaseMutex();
                mutex.WaitOne();
            }
            return mutex;
        }
    }
}
