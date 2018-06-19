using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ModuleTitleAttribute : TitleAttribute
    {
        public ModuleTitleAttribute(string title) : base(title) { } 

        public string TranslatedTitle()
        {
            return Title;
        }

        public override object Value => TranslatedTitle();
    }

   
}
