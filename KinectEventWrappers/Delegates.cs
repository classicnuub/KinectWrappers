using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectEventWrappers
{
    class Delegates
    {
        public delegate void CrossedArms(KinectEventsEnum kinectEvntEnum);
        public delegate void UnCrossedArms(KinectEventsEnum kinectEvntEnum);
        public delegate void RaiseHand(KinectEventsEnum kinectEvntEnum);
        public delegate void LowerHand(KinectEventsEnum kinectEvntEnum);
    }
}
