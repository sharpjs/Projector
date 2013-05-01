namespace Projector
{
    using System.Collections;
    using System.Collections.Generic;

    public static class CollectionTestExtensions
    {
        public static IList<T> EnumerateGeneric<T>(this IEnumerable<T> source)
        {
            // Can't use LINQ, because it doesn't always call GetEnumerator()

            if (source == null)
                throw Error.ArgumentNull("source");

            var items = new List<T>();

            foreach (T item in source)
                items.Add(item);

            return items;
        }

        public static IList<object> EnumerateNongeneric(this IEnumerable source)
        {
            // Can't use LINQ, because it doesn't always call GetEnumerator()

            if (source == null)
                throw Error.ArgumentNull("source");

            var items = new List<object>();

            foreach (object item in source)
                items.Add(item);

            return items;
        }
    }
}
