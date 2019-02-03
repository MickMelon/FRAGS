using System;

namespace Frags.Presentation.Attributes
{
    /// <summary>
    /// Marks whether a class is a ViewModel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelAttribute : Attribute
    {
        /// <summary>
        /// Checks if the given object has the ViewModel attribute.
        /// </summary>
        /// <param name="obj">The potential ViewModel</param>
        /// <returns>Whether the input has the ViewModel attribute.</returns>
        public static bool IsViewModel(Object obj) =>
            (obj != null) && (Attribute.IsDefined(obj.GetType(), typeof(ViewModelAttribute)));            
    }
}