using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    /// <summary>
    /// Exntension Methods on Collections
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Splits a collection in subcollections with a given size
        /// </summary>
        /// <typeparam name="T">the type of the collection's content</typeparam>
        /// <param name="instance">the collection to split</param>
        /// <param name="chunkSize">the size of the subcolleciton</param>
        /// <returns>a list of chunks with a maximum size of the value provided (chunksize)</returns>
        public static List<List<T>> Split<T>(this IEnumerable<T> instance, int chunkSize)
        {
            List<List<T>> retVal = new List<List<T>>();
            for (int i = 0; i < instance.Count(); i += chunkSize)
            {
                retVal.Add(instance.ToList().GetRange(i, Math.Min(chunkSize, instance.Count() - i)));
            }

            return retVal;
        }

        /// <summary>
        /// Converts a Collection of Collections back to one Collection 
        /// (e.g. to chunk it again with differen size)
        /// </summary>
        /// <typeparam name="T">the type of the collection's content</typeparam>
        /// <param name="instance">the collection to flatten</param>
        /// <returns>A collection containing all items of the previous subcollections</returns>
        public static List<T> ToFlatList<T>(this IEnumerable<IEnumerable<T>> instance)
        {
            List<T> retVal = new List<T>();

            foreach (var innerList in instance)
            {
                retVal.AddRange(innerList);
            }

            return retVal;
        }

        public static IEnumerable<T> Foreach<T>(this IEnumerable<T> instance, Action<T> action)
        {
            foreach (T item in instance ?? new T[] { })
            {
                action?.Invoke(item);
            }
            return instance;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> instance, params T[] elements)
        {
            return instance?.Concat(elements.AsEnumerable());
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> instance, T element)
        {
            return instance?.Concat(new[] { element });
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T element, IEqualityComparer<T> comparer = null)
        {
            int i = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            foreach (T currentElement in enumerable)
            {
                if (comparer.Equals(currentElement, element))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }
    }
}
