using System;
using System.Threading;

namespace WindowsMemoryMappedFile
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
            return mutex.WaitOne();
        }

        public bool LockMutex(int waitInMilliseconds)
        {
            return mutex.WaitOne(waitInMilliseconds);
        }

        public void ReleaseMutex()
        {
            mutex.ReleaseMutex();
        }
    }
}
