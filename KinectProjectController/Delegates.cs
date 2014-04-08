
using System.Windows.Media.Imaging;
namespace KinectProjectControllers
{
    public delegate void SendMessage(byte[] request);
    public delegate void SendTextMessage(string message);
    public delegate void BitmapUpdated(byte[] bitmap, int width, int height);
}
