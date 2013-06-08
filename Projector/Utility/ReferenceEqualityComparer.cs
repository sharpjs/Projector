namespace Projector.Utility
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        internal static ReferenceEqualityComparer<T> instance;

        public static ReferenceEqualityComparer<T> Instance
        {
            get { return instance ?? (instance = new ReferenceEqualityComparer<T>()); }
        }

        public bool Equals(T x, T y)
        {
            return x as object == y as object;
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
