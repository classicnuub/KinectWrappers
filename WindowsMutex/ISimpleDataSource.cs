
namespace SimpleDataSource
{
    public interface ISimpleDataSource
    {
        bool RequiresLength { get; }
        void WriteToDataSource(byte[] data);
        byte[] ReadFromDataSource();
        void SetLength(long dataLength);
        bool StartDataSource();
        void Dispose();
    }
}
