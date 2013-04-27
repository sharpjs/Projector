namespace Projector.ObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(ProjectionTypeCollection.DebugView))]
    public class ProjectionTypeCollection : ICollection<ProjectionType>
    {
        internal static readonly ProjectionTypeCollection
            Empty = new ProjectionTypeCollection(0);

        private readonly Dictionary<Type, ProjectionType> types;

        internal ProjectionTypeCollection(int capacity)
        {
            types = new Dictionary<Type, ProjectionType>(capacity);
        }

        public int Count
        {
            get { return types.Count; }
        }

        bool ICollection<ProjectionType>.IsReadOnly
        {
            get { return true; }
        }

        public bool Contains(Type type)
        {
            return types.ContainsKey(type);
        }

        public bool Contains(ProjectionType projectionType)
        {
            if (projectionType == null)
                throw Error.ArgumentNull("projectionType");

            ProjectionType candidate;
            return types.TryGetValue(projectionType.UnderlyingType, out candidate)
                && candidate == projectionType;
        }

        public bool TryGet(Type type, out ProjectionType projectionType)
        {
            return types.TryGetValue(type, out projectionType);
        }

        public ProjectionType this[Type type]
        {
            get { return types[type]; }
        }

        public void CopyTo(ProjectionType[] array, int index)
        {
            types.Values.CopyTo(array, index);
        }

        public IEnumerator<ProjectionType> GetEnumerator()
        {
            return types.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return types.Values.GetEnumerator();
        }

        internal void Add(ProjectionType projectionType)
        {
            types.Add(projectionType.UnderlyingType, projectionType);
        }

        void ICollection<ProjectionType>.Add(ProjectionType projectionType)
        {
            throw Error.ReadOnlyCollection();
        }

        void ICollection<ProjectionType>.Clear()
        {
            throw Error.ReadOnlyCollection();
        }

        bool ICollection<ProjectionType>.Remove(ProjectionType projectionType)
        {
            throw Error.ReadOnlyCollection();
        }

        internal sealed class DebugView
        {
            private readonly ProjectionTypeCollection collection;

            public DebugView(ProjectionTypeCollection collection)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                this.collection = collection;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ProjectionType[] Items
            {
                get
                {
                    var array = new ProjectionType[collection.Count];
                    collection.CopyTo(array, 0);
                    return array;
                }
            }
        }
    }
}
