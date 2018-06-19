
using PriceGrabber.Core;

namespace PriceGrabber.DependencyServices
{
    public interface IAPIHelper
    {
        string GetAppVersion();
        string GetDeviceModel();
        void RequestPermissions(string permission);
        void StartRequestLocation();
        void StopRequestLocation();
        GeoLocation GetCurrentLocation();
    }
}
