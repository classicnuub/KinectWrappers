using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDataSource;

namespace KinectUnitTest
{
    class SampleSimpleDataSource : ISimpleDataSource
    {
        public bool RequiresLength { get{ return false; } }

        public void WriteToDataSource(byte[] Data)
        {
            return;
        }

        public byte[] ReadFromDataSource()
        {
            return null;
        }

        public bool StartDataSource()
        {
            return true;
        }

        public void SetLength()
        {
        }

        public void Dispose()
        {
            return;
        }

        public void SetLength(long dataLength)
        {
        }
    }
}
