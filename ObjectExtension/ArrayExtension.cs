using System;

namespace ObjectExtension
{
    /// <summary>
    /// extension method for <see cref="Array"/>
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// The zero-based index of the first occurrence of value in the entire array, if found; otherwise, -1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] array, T value)
        {
            return Array.IndexOf(array, value);
        }
    }
}
