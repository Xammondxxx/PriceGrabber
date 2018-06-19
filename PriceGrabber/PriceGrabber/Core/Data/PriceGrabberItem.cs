using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core.Data
{
    public class PriceGrabberItem
    {
        public GeoLocation Location { get; set; }
        public string Country { get; set; }
        public string StoreName { get; set; }
        public string Comment { get; set; }
        public PriceGrabberProduct Product { get; set; }
    }

    public class PriceGrabberProduct
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string BrandName { get; set; }
    }
}
