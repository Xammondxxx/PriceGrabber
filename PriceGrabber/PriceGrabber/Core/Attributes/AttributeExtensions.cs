using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core.Attributes
{
    public static class AttributeExtensions
    {
        public enum Type
        {
            Field,
            Property
        }


        public static object GetTitleAttribute<T, A>(this object value, Type memberType)
            where T : IComparable
            where A : BaseAttribute
        {
            var type = typeof(T);

            var memInfo = GetmemberInfo(type, memberType, value);
            if (memInfo != null)
            {
                var attributes = memInfo.GetCustomAttributes(typeof(A), false);
                if (attributes != null)
                {
                    foreach (Attribute item in attributes)
                    {
                        if (item is A)
                            return (item as A).Value;
                    }
                }
            }
            return null;
        }

        private static MemberInfo GetmemberInfo(System.Type type, Type memberType, object value)
        {
            switch (memberType)
            {
                case Type.Field: return type.GetRuntimeField(value.ToString());
                case Type.Property: return type.GetRuntimeProperty(value.ToString());
            }

            return null;
        }




    }
}
