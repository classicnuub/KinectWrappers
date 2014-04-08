using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectEventWrappers;
using Microsoft.Kinect;

//********************************************************************************************************
//The majority of the color/depth gathering code is taken directly (with minor style changes) from the
//Kinect SDK Color and Depth Basics WPF projects.  They can be found in the Kinect SDK found at
//http://www.microsoft.com/en-us/kinectforwindows/.  -8/12/2012 Brandon Halverson
//********************************************************************************************************

namespace KinectEventWrappers
{
    public delegate void CrossedArms(KinectEventsEnum kinectEvntEnum);
    public delegate void UnCrossedArms(KinectEventsEnum kinectEvntEnum);
    public delegate void RaiseHand(KinectEventsEnum kinectEvntEnum);
    public delegate void LowerHand(KinectEventsEnum kinectEvntEnum);

    public class KinectEventWrapper : IKinectEventWrapper
    {
        #region Declarations
        private KinectSensor objKinectSensor;
        private bool crossed = false;
        private bool blnFoundSkeleton = false;
        private int crossedArmsCount;
        private bool rightHandRaised = false;
        private int rightHandRaisedCount;
        private bool leftHandRaised = false;
        private int leftHandRaisedCount;
        private int saveEventCount = 10;
        private int eventCountBetweenSaves = 10;
        private byte[] colorPixels;
        private byte[] depthDisplayPixels;
        private short[] depthPixels;
        private ColorImageFormat colorImgFormat = ColorImageFormat.RgbResolution640x480Fps30;
        private DepthImageFormat depthImgFormat = DepthImageFormat.Resolution640x480Fps30;

        public int ValidationCount { get; set; }
        public event EventHandler ColorBitmapChanged;
        public event EventHandler DepthBitmapChanged;
        public event EventHandler FoundSkeleton;

        public int DepthFrameWidth
        {
            get
            {
                if (objKinectSensor != null)
                    return objKinectSensor.DepthStream.FrameWidth;
                else
                    return 0;
            }
        }
        public int DepthFrameHeight
        {
            get
            {
                if (objKinectSensor != null)
                    return objKinectSensor.DepthStream.FrameHeight;
                else
                    return 0;
            }
        }
        public int ColorFrameWidth
        {
            get
            {
                if (objKinectSensor != null)
                    return objKinectSensor.ColorStream.FrameWidth;
                else
                    return 0;
            }
        }
        public int ColorFrameHeight
        {
            get
            {
                if (objKinectSensor != null)
                    return objKinectSensor.ColorStream.FrameHeight;
                else
                    return 0;
            }
        }
        public int ColorFrameByteLength
        {
            get
            {
                if (objKinectSensor != null)
                    return objKinectSensor.ColorStream.FramePixelDataLength;
                else
                    return 0;
            }
        }
        public int DepthFrameByteLength
        {
            get
            {
                if (objKinectSensor != null)
                    return objKinectSensor.DepthStream.FramePixelDataLength * 4;
                else
                    return 0;
            }
        }
        
        
        public event CrossedArms CrossArms;
        public event UnCrossedArms UnCrossArms;
        public event RaiseHand RightHandRaised;
        public event LowerHand RightHandLowered;
        public event RaiseHand LeftHandRaised;
        public event LowerHand LeftHandLowered;

        //_skel stores the last valid skeleton found.  This will persist even after there is no longer a skeleton in the frame.
        private SkeletonWithMapping _skel;
	    #endregion

        #region Constructors/Destructors
        public KinectEventWrapper()
        {
            ValidationCount = 10;

            //Find the first connected Kinect.  If none are found, objKinectSensor will be null.
            objKinectSensor = KinectSensor.KinectSensors.Where(k => k.Status == KinectStatus.Connected).FirstOrDefault();
            
            //If a connected Kinect was found start the skeleton stream, subscribe to its frame ready event and set the tracking mode to seated.  Then set up
            //the arrays for the depth and color images.  Also subscribe to those frame ready events.
            if (objKinectSensor != null)
            {
                objKinectSensor.SkeletonStream.Enable();

                objKinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(objKinectSensor_SkeletonFrameReady);

                objKinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                objKinectSensor.SkeletonStream.EnableTrackingInNearRange = true;

                objKinectSensor.ColorStream.Enable(colorImgFormat);
                this.colorPixels = new byte[objKinectSensor.ColorStream.FramePixelDataLength];
                objKinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(objKinectSensor_ColorFrameReady);

                objKinectSensor.DepthStream.Enable(depthImgFormat);
                objKinectSensor.DepthStream.Range = DepthRange.Near;
                this.depthPixels = new short[objKinectSensor.DepthStream.FramePixelDataLength];
                this.depthDisplayPixels = new byte[0];
                objKinectSensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(objKinectSensor_DepthFrameReady);

            }

            _skel = new SkeletonWithMapping();
        }

        public KinectEventWrapper(ColorImageFormat colorFormat, DepthImageFormat depthFormat) : this()
        {
            colorImgFormat = colorFormat;
            depthImgFormat = depthFormat;
        }

        ~KinectEventWrapper()
        {
            if (objKinectSensor != null && objKinectSensor.Status == KinectStatus.Connected)
            {
                objKinectSensor.Stop();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Process the available skeleton frames.  Fires any relevant events that have occurred.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void objKinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skels = new Skeleton[0];

            using (SkeletonFrame skelFrame = e.OpenSkeletonFrame())
            {
                if (skelFrame != null)
                {
                    skels = new Skeleton[skelFrame.SkeletonArrayLength];
                    skelFrame.CopySkeletonDataTo(skels);
                }
            }

            //Get the first active or PositionOnly skeleton found.  For multiple users this approach will need to be expanded upon.
            Skeleton tmpSkel = skels.Where(s => s.TrackingState == SkeletonTrackingState.Tracked || s.TrackingState == SkeletonTrackingState.PositionOnly).FirstOrDefault();
            
            if (tmpSkel != null && !blnFoundSkeleton)
            {
                if (FoundSkeleton != null)
                {
                    FoundSkeleton(null, null);
                    blnFoundSkeleton = true;
                }
            }

            //record the skeleton information for polling every saveEventCount number of events.
            if (eventCountBetweenSaves - saveEventCount == 0)
            {
                eventCountBetweenSaves = 0;
                if (tmpSkel != null)
                    _skel = new SkeletonWithMapping(tmpSkel, objKinectSensor, colorImgFormat);
            }
            else
                eventCountBetweenSaves++;

            //If a skeleton is found, process its values.
            if (tmpSkel != null)
            {
                JointCollection joints = tmpSkel.Joints;

                //This is commented out because it was for testing.  It is being left in code as an example and for potential updating in the future.

                //I'm being selective in all of the events and only taking tracked events where the type of tracking is Tracked and not Inferred.

                //Check for crosses or uncrossed arms.
                //if (joints[JointType.WristLeft].TrackingState == JointTrackingState.Tracked &&
                //    joints[JointType.WristRight].TrackingState == JointTrackingState.Tracked)
                //{
                //    if (joints[JointType.WristLeft].Position.X > joints[JointType.WristRight].Position.X && !crossed)
                //    {
                //        crossedArmsCount++;

                //        if (crossedArmsCount >= ValidationCount)
                //        {
                //            crossed = true;
                //            if (CrossArms != null)
                //                CrossArms(KinectEventsEnum.CrossArms);
                //        }
                //    }
                //    else if (joints[JointType.WristLeft].Position.X <= joints[JointType.WristRight].Position.X && crossed)
                //    {
                //        crossedArmsCount--;

                //        if (crossedArmsCount <= 0)
                //        {
                //            crossed = false;
                //            if (UnCrossArms != null)
                //            {
                //                UnCrossArms(KinectEventsEnum.UnCrossArms);
                //            }
                //        }
                //    }
                //}

                ////Check for hands raised.
                //if (joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                //    joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked &&
                //    joints[JointType.ShoulderCenter].TrackingState == JointTrackingState.Tracked)
                //{
                //    if (joints[JointType.HandRight].Position.Y >= joints[JointType.ShoulderCenter].Position.Y && !rightHandRaised)
                //    {
                //        rightHandRaisedCount++;

                //        if (rightHandRaisedCount >= ValidationCount)
                //        {
                //            rightHandRaised = true;

                //            if (RightHandRaised != null)
                //            {
                //                RightHandRaised(KinectEventsEnum.RightHandRaised);
                //            }
                //        }
                //    }
                //    else if (joints[JointType.HandRight].Position.Y < joints[JointType.ShoulderCenter].Position.Y && rightHandRaised)
                //    {
                //        rightHandRaisedCount--;

                //        if (rightHandRaisedCount <= 0)
                //        {
                //            rightHandRaised = false;

                //            if (RightHandLowered != null)
                //            {
                //                RightHandLowered(KinectEventsEnum.RightHandLowered);
                //            }
                //        }
                //    }

                //    if (joints[JointType.HandLeft].Position.Y >= joints[JointType.ShoulderCenter].Position.Y && !leftHandRaised)
                //    {
                //        leftHandRaisedCount++;

                //        if (leftHandRaisedCount >= ValidationCount)
                //        {
                //            leftHandRaised = true;

                //            if (LeftHandRaised != null)
                //            {
                //                LeftHandRaised(KinectEventsEnum.LeftHandRaised);
                //            }
                //        }
                //    }
                //    else if (joints[JointType.HandLeft].Position.Y < joints[JointType.ShoulderCenter].Position.Y && leftHandRaised)
                //    {
                //        leftHandRaisedCount--;

                //        if (leftHandRaisedCount <= 0)
                //        {
                //            leftHandRaised = false;

                //            if (LeftHandLowered != null)
                //            {
                //                LeftHandLowered(KinectEventsEnum.LeftHandLowered);
                //            }
                //        }
                //    }
                //}
            }
        }

        private void objKinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colFrame = e.OpenColorImageFrame())
            {
                if (colFrame != null)
                {
                    lock (colorPixels)
                    {
                        //Populate pixel array.
                        colFrame.CopyPixelDataTo(this.colorPixels);
                    }
                }
            }
        }

        private void objKinectSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    int bytesPerPixel = 4;
                    byte[] depthBytes = new byte[depthFrame.Width * depthFrame.Height * bytesPerPixel];
                    lock (this.depthPixels)
                    {
                        // Copy the pixel data from the image to a temporary array
                        depthFrame.CopyPixelDataTo(this.depthPixels);
                    }

                    for (int i = 0, j = 0; i < this.depthPixels.Length; i++, j += bytesPerPixel)
                    {
                        //Only show data for players, ignore background depths.
                        if ((depthPixels[i] & DepthImageFrame.PlayerIndexBitmask) == 0)
                        {
                            depthBytes[j] = 0;
                            depthBytes[j + 1] = 0;
                            depthBytes[j + 2] = 0;
                        }
                        else
                        {
                            // discard the portion of the depth that contains only the player index
                            int gray = ((int)(depthPixels[i] >> DepthImageFrame.PlayerIndexBitmaskWidth));
                            depthBytes[j] = (byte)(255 * gray / 0xFFF);
                            depthBytes[j + 1] = (byte)(255 * gray / 0xFFF);
                            depthBytes[j + 2] = (byte)(255 * gray / 0xFFF);
                        }
                    }

                    lock (this.depthDisplayPixels)
                    {
                        depthDisplayPixels = depthBytes;
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Enables the KinectSensor if one has been found.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                if (objKinectSensor != null && objKinectSensor.Status == KinectStatus.Connected)
                {
                    objKinectSensor.Start();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Stop the KinectSensor if one has been found.
        /// </summary>
        public void Stop()
        {
            if (objKinectSensor != null && objKinectSensor.Status == KinectStatus.Connected)
            {
                objKinectSensor.Stop();
            }
        }

        /// <summary>
        /// Return a parsable string for the skeleton joint and location data.
        /// </summary>
        /// <returns></returns>
        public string GetSkeletonJointsDataAsString()
        {
            return _skel.SkeletonToString();
        }

        /// <summary>
        /// Returns the current pixel data from the color stream.  If this.colorPixels is not instantiated, returns null.
        /// </summary>
        /// <returns></returns>
        public byte[] GetColorBytes()
        {
            if (this.colorPixels != null)
            {
                byte[] copyOfColorArray = new byte[this.colorPixels.Length];

                lock (this.colorPixels)
                {
                    this.colorPixels.CopyTo(copyOfColorArray, 0);
                }

                return copyOfColorArray;

                //Sample grayscale code.
                //int grayScaleBitCount = (int)Math.Floor((decimal)colorPixels.Length / (decimal) 3);
                //byte[] grayscale = new byte[grayScaleBitCount];

                //for (int i = 0; i < copyOfColorArray.Length; i += 3)
                //{
                //    //Average the value of rgb for a grayscale value.
                //    grayscale[i / 3] = BitConverter.GetBytes((Convert.ToInt32(copyOfColorArray[i]) + Convert.ToInt32(copyOfColorArray[i + 1]) + Convert.ToInt32(copyOfColorArray[i + 2])) / 3)[0];

                //    //Potential way to accomplish greyscale using a recommended ratio for each color.  Assuming the byte order is R,G,B
                //    //grayscale[i / 3] = BitConverter.GetBytes(Convert.ToInt32(.229 * copyOfColorArray[i] + .587 * copyOfColorArray[i + 1] + .114 * copyOfColorArray[i + 2]))[0];
                //}

                //return copyOfColorArray;
            }
            else
                return null;
        }

        /// <summary>
        /// Returns the current pixel data from the depth stream.  If this.depthDisplayPixels is not instantiated, returns null.
        /// </summary>
        /// <returns></returns>
        public byte[] GetDepthBytes()
        {
            if (this.depthDisplayPixels != null)
            {
                byte[] returnArray = new byte[this.depthDisplayPixels.Length];
                lock (this.depthDisplayPixels)
                {
                    this.depthDisplayPixels.CopyTo(returnArray, 0);
                }

                return returnArray;
            }
            else 
                return null;
        }

        /// <summary>
        /// Returns a copy of the depthPixels data currently stored.
        /// </summary>
        /// <returns></returns>
        public short[] GetRawDepthData()
        {
            short[] retArray = new short[depthPixels.Length];
            lock (this.depthPixels)
            {
                this.depthPixels.CopyTo(retArray, 0);
            }

            return retArray;
        }

        public void SetSittingSekeleton()
        {
            if (objKinectSensor != null)
            {
                objKinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            }
        }

        public void SetStandingSekeleton()
        {
            if (objKinectSensor != null)
            {
                objKinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }
        }

        public void SetCloseDistance()
        {
            if (objKinectSensor != null)
            {
                objKinectSensor.DepthStream.Range = DepthRange.Near;
            }
        }

        public void SetFarDistance()
        {
            if (objKinectSensor != null)
            {
                objKinectSensor.DepthStream.Range = DepthRange.Default;
            }
        }

        #endregion
    }
}
