using PriceGrabber.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.DependencyServices
{
    public interface IOrientationService
    {

        Orientation CurrentScreenOrientation { get; }
        Orientation CurrentDeviceOrientation { get; }

        //set orientation and forbid user to change orientation
        void LockOrientation(Orientation newOrientation);

        //set a base orientation and allow user to change orientation
        void ResetOrientation(Orientation baseOrientation);

    }
}
