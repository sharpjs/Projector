namespace Projector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class CollectionExtensions
    {
        public static IEnumerable<T> AsEnumerableSafe<T>(this IEnumerable<T> source)
        {
            return source
                ?? Enumerable.Empty<T>();
        }

        // Creates an array containing the unique items of a <source> enumerable, preserving order.
        // Returns null if source is null or empty.
        //
        public static T[] ToUniqueArrayOrNull<T>(this ICollection<T> source, IEqualityComparer<T> comparer)
        {
            // Assume source is probably small and already unique
            if (source == null)
                return null;

            var count = source.Count;
            if (count == 0)
                return null;

            var array = new T[count];
            source.CopyTo(array, 0);
            if (count == 1)
                return array;

            var set = new HashSet<T>(comparer);
            for (var i = 0; i < array.Length; i++)
                if (!set.Add(array[i]))
                    return BuildUniqueArray(array, set, i);

            return array;
        }

        // Creates an array containing the unique items of a <source> array, preserving order.
        // This is the fallback method, beginning the first non-unique item (at index <i>).
        //
        private static T[] BuildUniqueArray<T>(T[] source, HashSet<T> set, int i)
        {
            var count = i;
            source[i] = default(T);

            // Consolidate unique items at the front of the source array.
            for (i++; i < source.Length; i++)
            {
                var item  = source[i];
                source[i] = default(T);
                if (set.Add(item))
                    source[count++] = item;
            }

            // Copy to correctly-sized array
            var result = new T[count];
            Array.Copy(source, 0, result, 0, count);
            return result;
        }
    }
}
