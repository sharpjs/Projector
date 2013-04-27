namespace Projector.ObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(ProjectionPropertyCollection.DebugView))]
    public class ProjectionPropertyCollection : ICollection<ProjectionProperty>
    {
        internal static readonly ProjectionPropertyCollection
            Empty = new ProjectionPropertyCollection(0);

        private readonly HashSet   <           ProjectionProperty> properties;
        private readonly Dictionary<string,    ProjectionProperty> implicitProperties;
        private readonly Dictionary<MemberKey, ProjectionProperty> explicitProperties;

        internal ProjectionPropertyCollection(int capacity)
        {
            properties         = new HashSet   <           ProjectionProperty>();
            implicitProperties = new Dictionary<string,    ProjectionProperty>(capacity, StringComparer   .Ordinal );
            explicitProperties = new Dictionary<MemberKey, ProjectionProperty>(capacity, MemberKeyComparer.Instance);
        }

        public int Count
        {
            get { return properties.Count; }
        }

        bool ICollection<ProjectionProperty>.IsReadOnly
        {
            get { return true; }
        }

        public bool Contains(string name)
        {
            return implicitProperties.ContainsKey(name);
        }

        public bool Contains(string name, Type declaringType)
        {
            return explicitProperties.ContainsKey(new MemberKey(name, declaringType));
        }

        public bool Contains(ProjectionProperty property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            return properties.Contains(property);
        }

        public void CopyTo(ProjectionProperty[] array, int index)
        {
            properties.CopyTo(array, index);
        }

        public IEnumerator<ProjectionProperty> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        public ProjectionProperty this[string name]
        {
            get { return implicitProperties[name]; }
        }

        public ProjectionProperty this[string name, Type declaringType]
        {
            get { return explicitProperties[new MemberKey(name, declaringType)]; }
        }

        public ProjectionProperty this[ProjectionProperty property]
        {
            get { return explicitProperties[new MemberKey(property)]; }
        }

        public bool TryGet(string name, out ProjectionProperty property)
        {
            return implicitProperties.TryGetValue(name, out property);
        }

        public bool TryGet(string name, Type declaringType, out ProjectionProperty property)
        {
            return explicitProperties.TryGetValue(new MemberKey(name, declaringType), out property);
        }

        public bool TryGet(ref ProjectionProperty property)
        {
            return explicitProperties.TryGetValue(new MemberKey(property), out property);
        }

        internal void Add(ProjectionProperty property, bool declared)
        {
            if (properties.Add(property))
            {
                var key = new MemberKey(property);
                explicitProperties.Add(key, property);

                var name = key.MemberName;
                if (declared || implicitProperties.Remove(name) == false)
                    implicitProperties[name] = property;
            }
        }

        internal ProjectionProperty Override(string name, Type declaringType, ProjectionProperty newProperty)
        {
            // TODO: Refactor? TryGet will also create oldKey. Maybe just do it once?
            ProjectionProperty oldProperty;
            if (TryGet(name, declaringType, out oldProperty))
            {
                properties.Remove(oldProperty);

                var oldKey = new MemberKey(oldProperty);
                explicitProperties[oldKey] = newProperty;

                var oldName = oldKey.MemberName;
                if (oldName != newProperty.Name)
                    if (implicitProperties.ContainsKey(oldName))
                        implicitProperties[oldName] = newProperty;
                //else, implicitProperties should be updated by Add()

                return oldProperty;
            }

            throw Error.InvalidOverride(name, declaringType, newProperty);
        }

        void ICollection<ProjectionProperty>.Add(ProjectionProperty property)
        {
            throw Error.ReadOnlyCollection();
        }

        bool ICollection<ProjectionProperty>.Remove(ProjectionProperty property)
        {
            throw Error.ReadOnlyCollection();
        }

        void ICollection<ProjectionProperty>.Clear()
        {
            throw Error.ReadOnlyCollection();
        }

        internal sealed class DebugView
        {
            private readonly ProjectionPropertyCollection collection;

            public DebugView(ProjectionPropertyCollection collection)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                this.collection = collection;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ProjectionProperty[] Items
            {
                get
                {
                    var array = new ProjectionProperty[collection.Count];
                    collection.CopyTo(array, 0);
                    return array;
                }
            }
        }
    }
}
