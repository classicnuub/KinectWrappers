using System;
using System.Configuration;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows.Media.Imaging;
using KinectEventWrappers;
using KinectWPFGUI;

namespace KinectProjectControllers
{

    /// <summary>
    /// The goal of this class is to provide a central place to send and receive information to and from the Kinect Sensor.
    /// </summary>
    public class KinectProjectController : IKinectProjectController
    {
        #region Declarations

        private IKinectEventWrapper kWrapper;
        private string MemoryMappedFileName;
        private string MutexName;
        private MemoryMappedFile mmf;
        private Mutex mutex;
        private const long lng = 2458624;
        private Timer updateTimer;
        private TimerCallback updateTimerCallback;
        private long timerTick = 10;

        public WriteableBitmap ColorBitmap { get; set; }
        public WriteableBitmap DepthBitmap { get; set; }
        public event SendMessage sendMessage;
        public event SendTextMessage sendErrorMessage;
        public event EventHandler ColorImageChanged;
        public event EventHandler DepthImageChanged;
        public event EventHandler FoundSkeleton;

        #endregion Declarations

        #region Constructors

        public KinectProjectController(IKinectEventWrapper kinectEventWrapper)
        {
            kWrapper = kinectEventWrapper;

            //Set event handling for updating the image values.
            kWrapper.ColorBitmapChanged += new EventHandler(kWrapper_ColorBitmapChanged);
            kWrapper.DepthBitmapChanged += new EventHandler(kWrapper_DepthBitmapChanged);
            kWrapper.FoundSkeleton += kWrapper_FoundSkeleton;

            //Star the Kinect Wrapper class
            kWrapper.Start();

            //Assign a reference to the bitmaps that will be updating on the KinectEventWrapper
            ColorBitmap = kWrapper.colorBitmap;
            DepthBitmap = kWrapper.depthBitmap;

            //Start the timer for polling the KinectSensor data
            updateTimerCallback = updateTimer_Callback;
            updateTimer = new Timer(updateTimerCallback, null, 0, timerTick);
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Sends a command to close the kinect sensor to the wrapper class
        /// </summary>
        public void Close()
        {
            if (kWrapper != null)
                kWrapper.Stop();

            if (mmf != null)
                mmf.Dispose();
        }

        public byte[] ReceivedRequest(string request)
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
        public byte[] ReceivedRequest(ReceivedRequestEnum request)
        {
            string skelData = string.Empty;
            byte[] skeletonBytes;
            byte[] colorBytes;
            byte[] depthBytes;

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
                    colorBytes = kWrapper.GetColorBytes();
                    depthBytes = kWrapper.GetDepthBytes();
                    skelData = (kWrapper.GetSkeletonJointsDataAsString() + "|").PadRight(1024, '-');
                    skeletonBytes = System.Text.Encoding.ASCII.GetBytes(skelData);
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

                case ReceivedRequestEnum.skeleton:
                    skelData = (kWrapper.GetSkeletonJointsDataAsString() + "|").PadRight(1024, '-');
                    retValue = System.Text.Encoding.ASCII.GetBytes(skelData);
                    break;

                default:
                    retValue = System.Text.Encoding.ASCII.GetBytes("Invalid request received.");
                    break;
            }

            return retValue;
        }
        #endregion Methods

        #region Events
        /// <summary>
        /// When the reference for the color bitmap changes on the kinect event wrapper, get a reference to the
        /// new object and fire an event notifying the object has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void kWrapper_ColorBitmapChanged(object sender, EventArgs e)
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
        void kWrapper_DepthBitmapChanged(object sender, EventArgs e)
        {
            DepthBitmap = kWrapper.depthBitmap;

            if (DepthImageChanged != null)
                DepthImageChanged(null, null);
        }

        void kWrapper_FoundSkeleton(object sender, EventArgs e)
        {
            if (FoundSkeleton != null)
                FoundSkeleton(null, new EventArgs());
        }

        private void updateTimer_Callback(Object obj)
        {
        }
        #endregion Events
    }
}
