using Microsoft.Kinect;

namespace KinectEventWrappers
{
    public class SkeletonWithMapping
    {
        private Skeleton _skel;
        private ColorImagePoint _ankleLeft = new ColorImagePoint();
        private ColorImagePoint _ankleRight = new ColorImagePoint();
        private ColorImagePoint _elbowLeft = new ColorImagePoint();
        private ColorImagePoint _elbowRight = new ColorImagePoint();
        private ColorImagePoint _footLeft = new ColorImagePoint();
        private ColorImagePoint _footRight = new ColorImagePoint();
        private ColorImagePoint _handLeft = new ColorImagePoint();
        private ColorImagePoint _handRight = new ColorImagePoint();
        private ColorImagePoint _head = new ColorImagePoint();
        private ColorImagePoint _hipCenter = new ColorImagePoint();
        private ColorImagePoint _hipLeft = new ColorImagePoint();
        private ColorImagePoint _hipRight = new ColorImagePoint();
        private ColorImagePoint _kneeLeft = new ColorImagePoint();
        private ColorImagePoint _kneeRight = new ColorImagePoint();
        private ColorImagePoint _shoulderCenter = new ColorImagePoint();
        private ColorImagePoint _shoulderLeft = new ColorImagePoint();
        private ColorImagePoint _shoulderRight = new ColorImagePoint();
        private ColorImagePoint _skelPosition;
        private ColorImagePoint _spine = new ColorImagePoint();
        private ColorImagePoint _wristLeft = new ColorImagePoint();
        private ColorImagePoint _wristRight = new ColorImagePoint();

        public SkeletonWithMapping()
        {
            _skel = new Skeleton();
            _ankleLeft = new ColorImagePoint();
            _ankleRight = new ColorImagePoint();
            _elbowLeft = new ColorImagePoint();
            _elbowRight = new ColorImagePoint();
            _footLeft = new ColorImagePoint();
            _footRight = new ColorImagePoint();
            _handLeft = new ColorImagePoint();
            _handRight = new ColorImagePoint();
            _head = new ColorImagePoint();
            _hipCenter = new ColorImagePoint();
            _hipLeft = new ColorImagePoint();
            _hipRight = new ColorImagePoint();
            _kneeLeft = new ColorImagePoint();
            _kneeRight = new ColorImagePoint();
            _shoulderCenter = new ColorImagePoint();
            _shoulderLeft = new ColorImagePoint();
            _shoulderRight = new ColorImagePoint();
            _skelPosition = new ColorImagePoint();
            _spine = new ColorImagePoint();
            _wristLeft = new ColorImagePoint();
            _wristRight = new ColorImagePoint();
        }

        public SkeletonWithMapping(Skeleton skel)
        {
            _skel = skel;
        }

        public SkeletonWithMapping(Skeleton skel, KinectSensor objKinectSensor, ColorImageFormat imgFormat) : this(skel)
        {
            if (objKinectSensor != null)
            {
                _ankleLeft = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.AnkleLeft].Position, imgFormat);
                _ankleRight = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.AnkleRight].Position, imgFormat);
                _elbowLeft = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.ElbowLeft].Position, imgFormat);
                _elbowRight = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.ElbowRight].Position, imgFormat);
                _footLeft = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.FootLeft].Position, imgFormat);
                _footRight = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.FootRight].Position, imgFormat);
                _handLeft = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.HandLeft].Position, imgFormat);
                _handRight = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.HandRight].Position, imgFormat);
                _head = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.Head].Position, imgFormat);
                _hipCenter = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.HipCenter].Position, imgFormat);
                _hipLeft = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.HipLeft].Position, imgFormat);
                _hipRight = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.HipRight].Position, imgFormat);
                _kneeLeft = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.KneeLeft].Position, imgFormat);
                _kneeRight = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.KneeRight].Position, imgFormat);
                _shoulderCenter = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.ShoulderCenter].Position, imgFormat);
                _shoulderLeft = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.ShoulderLeft].Position, imgFormat);
                _shoulderRight = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.ShoulderRight].Position, imgFormat);
                _spine = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.Spine].Position, imgFormat);
                _wristLeft = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.WristLeft].Position, imgFormat);
                _wristRight = objKinectSensor.MapSkeletonPointToColor(skel.Joints[JointType.WristRight].Position, imgFormat);
            }
        }

        public string SkeletonToString()
        {
            if (_skel == null || _skel.Joints == null)
                return string.Empty;
            return
                "SkeletonPosition=" + _skel.Position.X + "," + _skel.Position.Y + "," + _skel.Position.Z + "," + _skelPosition.X + "," + _skelPosition.Y + "|" +
                JointType.AnkleLeft.ToString() + "=" + _skel.Joints[JointType.AnkleLeft].Position.X + "," + _skel.Joints[JointType.AnkleLeft].Position.Y + "," + _skel.Joints[JointType.AnkleLeft].Position.Z + "," + _ankleLeft.X + "," + _ankleLeft.Y + "|" +
                JointType.AnkleRight.ToString() + "=" + _skel.Joints[JointType.AnkleRight].Position.X + "," + _skel.Joints[JointType.AnkleRight].Position.Y + "," + _skel.Joints[JointType.AnkleRight].Position.Z + "," + _ankleRight.X + "," + _ankleRight.Y + "|" +
                JointType.ElbowLeft.ToString() + "=" + _skel.Joints[JointType.ElbowLeft].Position.X + "," + _skel.Joints[JointType.ElbowLeft].Position.Y + "," + _skel.Joints[JointType.ElbowLeft].Position.Z + "," + _elbowLeft.X + "," + _elbowLeft.Y + "|" +
                JointType.ElbowRight.ToString() + "=" + _skel.Joints[JointType.ElbowRight].Position.X + "," + _skel.Joints[JointType.ElbowRight].Position.Y + "," + _skel.Joints[JointType.ElbowRight].Position.Z + "," + _elbowRight.X + "," + _elbowRight.Y + "|" +
                JointType.FootLeft.ToString() + "=" + _skel.Joints[JointType.FootLeft].Position.X + "," + _skel.Joints[JointType.FootLeft].Position.Y + "," + _skel.Joints[JointType.FootLeft].Position.Z + "," + _footLeft.X + "," + _footLeft.Y + "|" +
                JointType.FootRight.ToString() + "=" + _skel.Joints[JointType.FootRight].Position.X + "," + _skel.Joints[JointType.FootRight].Position.Y + "," + _skel.Joints[JointType.FootRight].Position.Z + "," + _footRight.X + "," + _footRight.Y + "|" +
                JointType.HandLeft.ToString() + "=" + _skel.Joints[JointType.HandLeft].Position.X + "," + _skel.Joints[JointType.HandLeft].Position.Y + "," + _skel.Joints[JointType.HandLeft].Position.Z + "," + _handLeft.X + "," + _handLeft.Y + "|" +
                JointType.HandRight.ToString() + "=" + _skel.Joints[JointType.HandRight].Position.X + "," + _skel.Joints[JointType.HandRight].Position.Y + "," + _skel.Joints[JointType.HandRight].Position.Z + "," + _handRight.X + "," + _handRight.Y + "|" +
                JointType.Head.ToString() + "=" + _skel.Joints[JointType.Head].Position.X + "," + _skel.Joints[JointType.Head].Position.Y + "," + _skel.Joints[JointType.Head].Position.Z + "," + _head.X + "," + _head.Y + "|" +
                JointType.HipCenter.ToString() + "=" + _skel.Joints[JointType.HipCenter].Position.X + "," + _skel.Joints[JointType.HipCenter].Position.Y + "," + _skel.Joints[JointType.HipCenter].Position.Z + "," + _hipCenter.X + "," + _hipCenter.Y + "|" +
                JointType.HipLeft.ToString() + "=" + _skel.Joints[JointType.HipLeft].Position.X + "," + _skel.Joints[JointType.HipLeft].Position.Y + "," + _skel.Joints[JointType.HipLeft].Position.Z + "," + _hipLeft.X + "," + _hipLeft.Y + "|" +
                JointType.HipRight.ToString() + "=" + _skel.Joints[JointType.HipRight].Position.X + "," + _skel.Joints[JointType.HipRight].Position.Y + "," + _skel.Joints[JointType.HipRight].Position.Z + "," + _hipRight.X + "," + _hipRight.Y + "|" +
                JointType.KneeLeft.ToString() + "=" + _skel.Joints[JointType.KneeLeft].Position.X + "," + _skel.Joints[JointType.KneeLeft].Position.Y + "," + _skel.Joints[JointType.KneeLeft].Position.Z + "," + _kneeLeft.X + "," + _kneeLeft.Y + "|" +
                JointType.KneeRight.ToString() + "=" + _skel.Joints[JointType.KneeRight].Position.X + "," + _skel.Joints[JointType.KneeRight].Position.Y + "," + _skel.Joints[JointType.KneeRight].Position.Z + "," + _kneeRight.X + "," + _kneeRight.Y + "|" +
                JointType.ShoulderCenter.ToString() + "=" + _skel.Joints[JointType.ShoulderCenter].Position.X + "," + _skel.Joints[JointType.ShoulderCenter].Position.Y + "," + _skel.Joints[JointType.ShoulderCenter].Position.Z + "," + _shoulderCenter.X + "," + _shoulderCenter.Y + "|" +
                JointType.ShoulderLeft.ToString() + "=" + _skel.Joints[JointType.ShoulderLeft].Position.X + "," + _skel.Joints[JointType.ShoulderLeft].Position.Y + "," + _skel.Joints[JointType.ShoulderLeft].Position.Z + "," + _shoulderLeft.X + "," + _shoulderLeft.Y + "|" +
                JointType.ShoulderRight.ToString() + "=" + _skel.Joints[JointType.ShoulderRight].Position.X + "," + _skel.Joints[JointType.ShoulderRight].Position.Y + "," + _skel.Joints[JointType.ShoulderRight].Position.Z + "," + _shoulderRight.X + "," + _shoulderRight.Y + "|" +
                JointType.Spine.ToString() + "=" + _skel.Joints[JointType.Spine].Position.X + "," + _skel.Joints[JointType.Spine].Position.Y + "," + _skel.Joints[JointType.Spine].Position.Z + "," + _spine.X + "," + _spine.Y + "|" +
                JointType.WristLeft.ToString() + "=" + _skel.Joints[JointType.WristLeft].Position.X + "," + _skel.Joints[JointType.WristLeft].Position.Y + "," + _skel.Joints[JointType.WristLeft].Position.Z + "," + _wristLeft.X + "," + _wristLeft.Y + "|" +
                JointType.WristRight.ToString() + "=" + _skel.Joints[JointType.WristRight].Position.X + "," + _skel.Joints[JointType.WristRight].Position.Y + "," + _skel.Joints[JointType.WristRight].Position.Z + "," + _wristRight.X + "," + _wristRight.Y;

        }
    }
}
