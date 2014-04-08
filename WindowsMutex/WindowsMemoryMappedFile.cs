using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using SimpleDataSource;

namespace WindowsMemoryMappedFileNS
{
    public class WindowsMemoryMappedFile : ISimpleDataSource
    {
        private bool _requiresLength = true;
        public bool RequiresLength { get { return _requiresLength; } }

        private WindowsMutex mutex;
        private MemoryMappedFile memoryMappedfile;
        private long memoryLength;
        private string memoryMappedFileName;

        public WindowsMemoryMappedFile(string mutexName, string memoryMappedFileName)
        {
            this.memoryMappedFileName = memoryMappedFileName;
            mutex = new WindowsMutex(mutexName);
        }

        public void WriteToDataSource(byte[] Data)
        {
            mutex.LockMutex();

            try
            {
                if (!memoryMappedfile.SafeMemoryMappedFileHandle.IsClosed)
                {
                    //Get a stream of the MemoryMappedFile
                    using (MemoryMappedViewStream mmStrm = memoryMappedfile.CreateViewStream())
                    {
                        //Write to the MemoryMappedFile
                        BinaryWriter bWrite = new BinaryWriter(mmStrm);
                        bWrite.Write(Data);
                    }
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            };
        }

        public byte[] ReadFromDataSource()
        {
            byte[] data = new byte[memoryLength];

            mutex.LockMutex();

            if (!memoryMappedfile.SafeMemoryMappedFileHandle.IsClosed)
            {
                //Get the data to display for testing from the memorymappedfile.
                using (MemoryMappedViewStream mmStrm = memoryMappedfile.CreateViewStream())
                {
                    BinaryReader bRead = new BinaryReader(mmStrm);
                    bRead.Read(data, 0, Convert.ToInt32(memoryLength));
                }
            }

            mutex.ReleaseMutex();

            return data;
        }

        public void SetLength(long length)
        {
            memoryLength = length;
        }

        public bool StartDataSource()
        {
            if (memoryLength > 0)
            {
                memoryMappedfile = MemoryMappedFile.CreateNew(memoryMappedFileName, memoryLength);

                return true;
            }
            else
                return false;
        }

        public void Dispose()
        {
            if (mutex != null)
                mutex.Dispose();

            if (memoryMappedfile != null)
                memoryMappedfile.Dispose();
        }
    }
}
