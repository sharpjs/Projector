namespace Projector.Utility
{
    using System.Collections.Generic;

    internal class ObjectEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        internal static ObjectEqualityComparer<T> instance;

        public static ObjectEqualityComparer<T> Instance
        {
            get { return instance ?? (instance = new ObjectEqualityComparer<T>()); }
        }

        public bool Equals(T x, T y)
        {
            return x == null ? y == null : x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }
}
