using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectEventWrappers;

namespace KinectUnitTest
{
    internal class TestKinectEventWrapper : IKinectEventWrapper
    {

        int IKinectEventWrapper.ValidationCount
        {
            get
            {
                return 10;
            }
            set
            {
            }
        }

        event EventHandler IKinectEventWrapper.ColorBitmapChanged
        {
            add {}
            remove {}
        }

        event EventHandler IKinectEventWrapper.DepthBitmapChanged
        {
            add { }
            remove { }
        }

        event EventHandler IKinectEventWrapper.FoundSkeleton
        {
            add { }
            remove { }
        }

        event CrossedArms IKinectEventWrapper.CrossArms
        {
            add { }
            remove { }
        }

        event UnCrossedArms IKinectEventWrapper.UnCrossArms
        {
            add { }
            remove { }
        }

        event RaiseHand IKinectEventWrapper.RightHandRaised
        {
            add { }
            remove { }
        }

        event LowerHand IKinectEventWrapper.RightHandLowered
        {
            add { }
            remove { }
        }

        event RaiseHand IKinectEventWrapper.LeftHandRaised
        {
            add { }
            remove { }
        }

        event LowerHand IKinectEventWrapper.LeftHandLowered
        {
            add { }
            remove { }
        }

        bool IKinectEventWrapper.Start()
        {
            return true;
        }

        void IKinectEventWrapper.Stop()
        {
            return;
        }

        string IKinectEventWrapper.GetSkeletonJointsDataAsString()
        {
            return "Skelly Data";
        }

        byte[] IKinectEventWrapper.GetColorBytes()
        {
            return null;
        }

        byte[] IKinectEventWrapper.GetDepthBytes()
        {
            return null;
        }

        short[] IKinectEventWrapper.GetRawDepthData()
        {
            return null;
        }

        void IKinectEventWrapper.SetSittingSekeleton()
        {
            return;
        }

        void IKinectEventWrapper.SetStandingSekeleton()
        {
            return;
        }

        void IKinectEventWrapper.SetCloseDistance()
        {
            return;
        }

        void IKinectEventWrapper.SetFarDistance()
        {
            return;
        }

        public int DepthFrameWidth
        {
            get { throw new NotImplementedException(); }
        }

        public int DepthFrameHeight
        {
            get { throw new NotImplementedException(); }
        }

        public int ColorFrameWidth
        {
            get { throw new NotImplementedException(); }
        }

        public int ColorFrameHeight
        {
            get { throw new NotImplementedException(); }
        }


        public int ColorFrameByteLength
        {
            get { throw new NotImplementedException(); }
        }

        public int DepthFrameByteLength
        {
            get { throw new NotImplementedException(); }
        }
    }
}
