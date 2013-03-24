namespace Projector
{
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class CollectionDebugView<T>
    {
        private readonly ICollection<T> collection;

        public CollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
                throw Error.ArgumentNull("collection");

            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }
        }
    }
}
