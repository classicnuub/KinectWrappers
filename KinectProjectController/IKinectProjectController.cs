using System;
using System.Windows.Media.Imaging;

namespace KinectProjectControllers
{
    /// <summary>
    /// Using an interface is probably overkill since this will never be inherited from by more than one class specifically for the process.  However, it can
    /// be useful for unit testing so I have left it in here as a good example to use for that reason.
    /// </summary>
    public interface IKinectProjectController
    {
        #region Methods
        byte[] ReceivedRequest(string request);
        byte[] ReceivedRequest(ReceivedRequestEnum request);
        #endregion Methods

        #region Declarations
        event EventHandler ColorImageChanged;
        event EventHandler DepthImageChanged;
        event EventHandler FoundSkeleton;
        event SendMessage sendMessage;
        event SendTextMessage sendErrorMessage;
        WriteableBitmap ColorBitmap { get; set; }
        WriteableBitmap DepthBitmap { get; set; }
        #endregion Declarations
    }
}
