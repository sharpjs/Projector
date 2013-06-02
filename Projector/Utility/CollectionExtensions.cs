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

        public static T[] ToArraySafe<T>(this IEnumerable<T> source)
        {
            return source == null
                ? new T[0]
                : source.ToArray();
        }
    }
}
