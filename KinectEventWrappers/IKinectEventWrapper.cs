using System;
using System.Windows.Media.Imaging;
using KinectEventWrappers;

namespace KinectEventWrappers
{
    public interface IKinectEventWrapper
    {
        #region Properties
        int ValidationCount { get; set; }
        event EventHandler ColorBitmapChanged;
        event EventHandler DepthBitmapChanged;
        event EventHandler FoundSkeleton;

        WriteableBitmap colorBitmap { get; set; }
        WriteableBitmap depthBitmap { get; set; }
        event CrossedArms CrossArms;
        event UnCrossedArms UnCrossArms;
        event RaiseHand RightHandRaised;
        event LowerHand RightHandLowered;
        event RaiseHand LeftHandRaised;
        event LowerHand LeftHandLowered;
        #endregion Properties

        #region Methods
        bool Start();

        void Stop();

        string GetSkeletonJointsDataAsString();

        byte[] GetColorBytes();

        byte[] GetDepthBytes();

        short[] GetRawDepthData();

        void SetSittingSekeleton();

        void SetStandingSekeleton();

        void SetCloseDistance();

        void SetFarDistance();
        #endregion Methods
    }
}
