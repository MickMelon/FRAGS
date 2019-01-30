using System;

namespace Frags.Core.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModel : Attribute
    {
        public static bool IsViewModel(Object obj) =>
            (obj != null) && (Attribute.IsDefined(obj.GetType(), typeof(ViewModel)));            
    }
}