namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Projector.Utility;
    using System.Linq;

    [DebuggerTypeProxy(typeof(ProjectionInstance.DebugView))]
    public sealed class ProjectionInstance : ProjectionObject
    {
        private readonly ProjectionFactory factory;
        private readonly object            context;

        private Cell<Projection>           projections;
        private Cell<ProjectionStorage>    storages;

        internal ProjectionInstance(ProjectionFactory factory, int token, object store, object context)
        {
            this.factory  = factory;
            this.context  = context;
            this.storages = Cell.Cons(new ProjectionStorage(store, token, null));
        }

        public override ProjectionFactory Factory
        {
            get { return factory; }
        }

        public object Context
        {
            get { return context; }
        }

        public IEnumerable<object> Stores
        {
            get { return storages.AsEnumerableSafe().Select(s => s.Store); }
        }

        internal Cell<ProjectionStorage> FirstStorage
        {
            get { return storages; }
        }

        public IEnumerable<Projection> Projections
        {
            get { return projections.AsEnumerableSafe(); }
        }

        internal Cell<Projection> FirstProjection
        {
            get { return projections; }
        }

        // Caching will be done in the emitted IL to private instance fields w/set-notset bitfield.
        // So the dictionary provider will be a real "dictionary for storage" behavior, not a cache.

        internal object Initialize(Type type, object arguments)
        {
            // Assume no existing projections
            Cell<IProjectionBehavior> firstBehavior;
            var projection = ProjectCore(type, out firstBehavior);
            projections = Cell.Cons(projection);

            new InstanceInitializerInvocation
                (projection, arguments, firstBehavior)
                .Proceed();

            return projection;
        }

        public T Project<T>()
        {
            return (T) Project(typeof(T));
        }

        public object Project(Type type)
        {
            Cell<Projection> cell = null, next = projections;
            Cell<IProjectionBehavior> firstBehavior;
            Projection projection;

            // Try to return an existing projection fist
            do
            {
                projection = next.Item;
                if (projection.Type.UnderlyingType == type)
                    return projection;

                cell = next;
                next = cell.Next;
            }
            while (next != null);

            // Else create new projection
            projection = ProjectCore(type, out firstBehavior);
            cell.Next = Cell.Cons(projection);

            new ProjectionInitializerInvocation
                (projection, firstBehavior)
                .Proceed();

            return projection;
        }

        private Projection ProjectCore(Type type, out Cell<IProjectionBehavior> firstBehavior)
        {
            var projectionType = factory.GetProjectionType(type) as ProjectionStructureType;
            if (projectionType == null)
                throw Error.InvalidProjectionType(type);

            firstBehavior = projectionType.FirstBehavior;
            return projectionType.CreateProjection(this);
        }

        private sealed class DebugView
        {
            private readonly ProjectionInstance instance;

            public DebugView(ProjectionInstance instance)
            {
                if (instance == null)
                    throw Error.ArgumentNull("instance");

                this.instance = instance;
            }

            public ProjectionFactory Factory
            {
                get { return instance.Factory; }
            }

            public object Context
            {
                get { return instance.Context; }
            }

            public object[] Stores
            {
                get { return instance.Stores.ToArray(); }
            }

            public Projection[] Projections
            {
                get { return instance.Projections.ToArray(); }
            }
        }
    }
}
