using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core
{
    [AttributeUsage(AttributeTargets.All)]
    public class TitleAttribute : BaseAttribute
    {
        public string Title { get; set; }
        public TitleAttribute(string title)
        {
            Title = title;
        }

        public override object Value => Title;
    }
}
