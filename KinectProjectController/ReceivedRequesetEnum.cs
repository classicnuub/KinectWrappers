using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectProjectControllers
{
    public enum ReceivedRequestEnum : int
    {
        invalid = 0,
        bytes = 1,
        color = 2,
        depth = 3,
        rawdepth = 4,
        all = 5,
        sit = 6,
        stand = 7,
        close = 8,
        far = 9,
        skeleton = 10
    }
}
