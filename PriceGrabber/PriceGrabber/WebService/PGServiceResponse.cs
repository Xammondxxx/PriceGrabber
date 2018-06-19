using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.WebService
{
    public class PGServiceResponse
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
        public bool Result { get; set; }
        public string Headers { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public string ETag { get; set; }
    }
}
