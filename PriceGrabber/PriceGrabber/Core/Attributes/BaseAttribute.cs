using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core
{
    public abstract class BaseAttribute :Attribute
    {
       public virtual object Value { get; }     
    }
}
