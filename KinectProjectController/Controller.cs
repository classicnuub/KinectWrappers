using System;
using System.Windows.Media.Imaging;
using KinectEventWrappers;
using KinectWrappers;

namespace KinectProjectControllers
{
    public delegate void SendMessage(byte[] request);

    public static class KinectProjectController
    {
        #region Declarations

        private static KinectEventWrapper kWrapper;

        public static WriteableBitmap ColorBitmap { get; set; }
        public static WriteableBitmap DepthBitmap { get; set; }
        public static event SendMessage sendMessage;
        public static event EventHandler ColorImageChanged;
        public static event EventHandler DepthImageChanged;
        public static event EventHandler FoundSkeleton;

        #endregion Declarations

        #region Constructors

        static KinectProjectController()
        {
            kWrapper = new KinectEventWrapper();

            //These events were used in testing.  Leaving them in place for examples in the future.
            //kWrapper.LeftHandRaised += new RaiseHand(kWrap_Event);
            //kWrapper.LeftHandLowered += new LowerHand(kWrap_Event);
            //kWrapper.RightHandRaised += new RaiseHand(kWrap_Event);
            //kWrapper.RightHandLowered += new LowerHand(kWrap_Event);

            kWrapper.ColorBitmapChanged += new EventHandler(kWrapper_ColorBitmapChanged);
            kWrapper.DepthBitmapChanged += new EventHandler(kWrapper_DepthBitmapChanged);
            kWrapper.FoundSkeleton += kWrapper_FoundSkeleton;

            kWrapper.Start();

            ColorBitmap = kWrapper.colorBitmap;
            DepthBitmap = kWrapper.depthBitmap;
        }

        #endregion Constructors

        #region Methods

        public static void Close()
        {
            if (kWrapper != null)
                kWrapper.Stop();
        }

        public static void SendMessageToAllClients(byte[] msg)
        {
            if (sendMessage != null)
                sendMessage(msg);
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Whenever an event is fired by the KinectWrapper class this method is called.  An event is sent to all clients
        /// when this occurrs.
        /// </summary>
        /// <param name="kEvntWrap"></param>
        private static void kWrap_Event(KinectEventsEnum kEvntWrap)
        {
            //Set a message string based on the event fired.
            string msg = "You got event: " + kEvntWrap.ToString();

            //Build the message.  The first byte will be the length of the message, the remainder will be the actual message.
            byte[] msgBytes = new byte[1 + msg.Length];

            //Set the first byte to notify the receiver of the length of the message.
            msgBytes[0] = (byte)(msg.Length);

            //Build the remainder of the message in the Byte array.
            int i = 1;
            foreach (byte bByte in System.Text.Encoding.ASCII.GetBytes(msg))
            {
                msgBytes[i++] = bByte;
            }

            SendMessageToAllClients(msgBytes);
        }

        /// <summary>
        /// When the reference for the color bitmap changes on the kinect event wrapper, get a reference to the
        /// new object and fire an event notifying the object has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void kWrapper_ColorBitmapChanged(object sender, EventArgs e)
        {
            ColorBitmap = kWrapper.colorBitmap;

            if (ColorImageChanged != null)
                ColorImageChanged(null, null);
        }


        /// <summary>
        /// When the reference for the depth bitmap changes on the kinect event wrapper, get a reference to the
        /// new object and fire an event notifying the object has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void kWrapper_DepthBitmapChanged(object sender, EventArgs e)
        {
            DepthBitmap = kWrapper.depthBitmap;

            if (DepthImageChanged != null)
                DepthImageChanged(null, null);
        }

        public static byte[] ReceivedRequest(string request)
        {
            ReceivedRequestEnum requestEnum = ReceivedRequestEnum.invalid;

            Enum.TryParse<ReceivedRequestEnum>(request, out requestEnum);

            return ReceivedRequest(requestEnum);
        }

        /// <summary>
        /// When a request is received, fire the event if it has any subscribers.
        /// 
        /// Then handle the request and respond.
        /// </summary>
        /// <param name="request"></param>
        public static byte[] ReceivedRequest(ReceivedRequestEnum request)
        {
            byte[] retValue = new byte[0];

            switch (request)
            {
                case ReceivedRequestEnum.bytes:
                    retValue = System.Text.Encoding.ASCII.GetBytes((kWrapper.GetSkeletonJointsDataAsString() + "|").PadRight(1024, '-'));
                    break;

                case ReceivedRequestEnum.color:
                    retValue = kWrapper.GetColorBytes();
                    break;

                case ReceivedRequestEnum.depth:
                    retValue = kWrapper.GetDepthBytes();
                    break;

                case ReceivedRequestEnum.rawdepth:
                    short[] rawShortData = kWrapper.GetRawDepthData();
                    retValue = new byte[rawShortData.Length * 2];
                    Buffer.BlockCopy(rawShortData, 0, retValue, 0, retValue.Length);
                    break;

                case ReceivedRequestEnum.all:
                    byte[] colorBytes = kWrapper.GetColorBytes();
                    byte[] depthBytes = kWrapper.GetDepthBytes();
                    string skelData = (kWrapper.GetSkeletonJointsDataAsString() + "|").PadRight(1024, '-');
                    byte[] skeletonBytes = System.Text.Encoding.ASCII.GetBytes(skelData);
                    retValue = new byte[colorBytes.Length + depthBytes.Length + skeletonBytes.Length];
                    Buffer.BlockCopy(colorBytes, 0, retValue, 0, colorBytes.Length);
                    Buffer.BlockCopy(depthBytes, 0, retValue, colorBytes.Length, depthBytes.Length);
                    Buffer.BlockCopy(skeletonBytes, 0, retValue, colorBytes.Length + depthBytes.Length, skeletonBytes.Length);
                    break;

                case ReceivedRequestEnum.sit:
                    kWrapper.SetSittingSekeleton();
                    break;

                case ReceivedRequestEnum.stand:
                    kWrapper.SetStandingSekeleton();
                    break;

                case ReceivedRequestEnum.close:
                    kWrapper.SetCloseDistance();
                    break;

                case ReceivedRequestEnum.far:
                    kWrapper.SetFarDistance();
                    break;

                default:
                    retValue = System.Text.Encoding.ASCII.GetBytes("Invalid request received.");
                    break;
            }

            return retValue;
        }

        static void kWrapper_FoundSkeleton(object sender, EventArgs e)
        {
            if (FoundSkeleton != null)
                FoundSkeleton(null, new EventArgs());
        }

        #endregion Methods
    }
}
