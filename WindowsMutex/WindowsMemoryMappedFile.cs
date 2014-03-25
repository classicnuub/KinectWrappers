using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace WindowsMemoryMappedFile
{
    public class WindowsMemoryMappedFile
    {
        private WindowsMutex mutex;
        private MemoryMappedFile memoryMappedfile;

        public WindowsMemoryMappedFile(string mutexName, string memoryMappedFileName, long memoryMappedFileLength)
        {
            mutex = new WindowsMutex(mutexName);

            memoryMappedfile = MemoryMappedFile.CreateNew(memoryMappedFileName, memoryMappedFileLength);
        }

        public void WriteToFile(byte[] Data)
        {
            mutex.LockMutex();

            try
            {
                //Get a stream of the MemoryMappedFile
                using (MemoryMappedViewStream mmStrm = memoryMappedfile.CreateViewStream())
                {
                    //Write to the MemoryMappedFile
                    BinaryWriter bWrite = new BinaryWriter(mmStrm);
                    bWrite.Write(Data);
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            };
        }

        public byte[] PullDataFromFile(long lengthToRead)
        {
            byte[] data = new byte[lengthToRead];

            mutex.LockMutex();

            //Get the data to display for testing from the memorymappedfile.
            using (MemoryMappedViewStream mmStrm = memoryMappedfile.CreateViewStream())
            {
                BinaryReader bRead = new BinaryReader(mmStrm);
                bRead.Read(data, 0, Convert.ToInt32(lengthToRead));
            }

            mutex.ReleaseMutex();

            return data;
        }
    }
}
