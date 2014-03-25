using System;
using KinectProjectControllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KinectUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestInvalidRequest()
        {
            KinectProjectController kpc = new KinectProjectController(new TestKinectEventWrapper());

            byte[] result = kpc.ReceivedRequest(ReceivedRequestEnum.invalid);

            Assert.AreEqual("Invalid request received.", System.Text.Encoding.ASCII.GetString(result));
        }

        [TestMethod]
        public void TestSkeletonRequest()
        {
            KinectProjectController kpc = new KinectProjectController(new TestKinectEventWrapper());

            byte[] result = kpc.ReceivedRequest(ReceivedRequestEnum.skeleton);

            Assert.AreEqual("Skelly Data|".PadRight(1024, '-'), System.Text.Encoding.ASCII.GetString(result));
        }
    }
}
