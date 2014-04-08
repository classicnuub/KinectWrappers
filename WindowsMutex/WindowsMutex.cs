using System;
using System.Threading;

namespace WindowsMemoryMappedFileNS
{
    internal class WindowsMutex
    {
        private Mutex mutex;

        public WindowsMutex(string mutexName)
        {
            bool createdNew = false;

            mutex = new Mutex(true, mutexName, out createdNew);

            if (createdNew)
            {
                mutex.ReleaseMutex();
            }
            else
            {
                throw new Exception("The mutex used to validate memory map locking was not able to be created.");
            }
        }

        public bool LockMutex()
        {
            if (!mutex.SafeWaitHandle.IsClosed)
                return mutex.WaitOne();
            else
                return false;
        }

        public bool LockMutex(int waitInMilliseconds)
        {
            if (!mutex.SafeWaitHandle.IsClosed)
                return mutex.WaitOne(waitInMilliseconds);
            else
                return false;
        }

        public void ReleaseMutex()
        {
            if (!mutex.SafeWaitHandle.IsClosed)
                mutex.ReleaseMutex();
        }

        public void Dispose()
        {
            if (mutex != null)
                mutex.Close();
        }
    }
}
