using System;
using System.Threading;
using KinectEventWrappers;
using SimpleDataSource;

namespace KinectProjectControllers
{

    /// <summary>
    /// The goal of this class is to provide a central place to send and receive information to and from the Kinect Sensor.
    /// </summary>
    public class KinectProjectController
    {
        #region Declarations
        private IKinectEventWrapper kWrapper;
        private ISimpleDataSource colorDataSource;
        private ISimpleDataSource depthDataSource;
        private Timer updateTimer;
        private long timerTick = 10;
        #endregion Declarations

        #region Constructors
        public KinectProjectController(IKinectEventWrapper kinectEventWrapper, ISimpleDataSource colorDataSource, ISimpleDataSource depthDataSource)
        {
            kWrapper = kinectEventWrapper;
            this.colorDataSource = colorDataSource;
            this.depthDataSource = depthDataSource;

            //Star the Kinect Wrapper class
            kWrapper.Start();

            if (colorDataSource.RequiresLength)
            {
                colorDataSource.SetLength(kWrapper.ColorFrameByteLength);
                colorDataSource.StartDataSource();
            }

            if (depthDataSource.RequiresLength)
            {
                depthDataSource.SetLength(kWrapper.DepthFrameByteLength);
                depthDataSource.StartDataSource();
            }

            //Start the timer for polling the KinectSensor data
            updateTimer = new Timer(updateTimer_Callback, null, 0, timerTick);
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

            if (colorDataSource != null)
                colorDataSource.Dispose();

            if (depthDataSource != null)
                depthDataSource.Dispose();
        }

        /// <summary>
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

        private void WriteToDataSource(byte[] Data, ISimpleDataSource dataSource)
        {
            if (dataSource != null)
                dataSource.WriteToDataSource(Data);
        }
        #endregion Methods

        #region Events
        private void updateTimer_Callback(Object obj)
        {
            colorDataSource.WriteToDataSource(kWrapper.GetColorBytes());
            depthDataSource.WriteToDataSource(kWrapper.GetDepthBytes());
        }
        #endregion Events
    }
}
