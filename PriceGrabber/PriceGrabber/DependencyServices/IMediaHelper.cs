using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.DependencyServices
{
    public interface IMediaHelper
    {
        byte[] ResizeImage(byte[] file, float maxWidth, float maxHeight);
    }
}
