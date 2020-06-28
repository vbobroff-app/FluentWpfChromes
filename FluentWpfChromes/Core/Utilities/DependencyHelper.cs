using System.Windows;
using System.Windows.Media;

namespace FluentWpfChromes
{
    /// <summary>
    /// Extensions for DependencyObject type
    /// </summary>
    internal static  class DependencyHelper
    {
        /// <summary>
        /// Finds a parent of a given control/item on the visual tree. 
        /// Use Recursive method
        /// </summary>
        /// <typeparam name="T">Type of Parent</typeparam>
        /// <param name="child">Child whose parent is queried</param>
        /// <returns>Returns the first parent item that matched the type (T), if no match found then it will return null</returns>
        public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parentObject = VisualTreeHelper.GetParent(child);
                switch (parentObject)
                {
                    case null:
                        return null;
                    case T parent:
                        return parent;
                    default:
                        child = parentObject;
                        continue;
                }
            }
        }
    }
}
