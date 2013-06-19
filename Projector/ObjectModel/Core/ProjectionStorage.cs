namespace Projector.ObjectModel
{
    using System;

    internal struct ProjectionStorage
    {
        private readonly object        store;
        private readonly int           token;
        private readonly WeakReference graph;

        public ProjectionStorage(object store, int token, object graph)
        {
            this.store = store;
            this.token = token;
            this.graph = graph != null ? new WeakReference(graph) : null;
        }

        public object Store
        {
            get { return store; }
        }

        public int Token
        {
            get { return token; }
        }

        internal bool IsAlive
        {
            get { return graph == null || graph.IsAlive; }
        }

        internal object Graph
        {
            get { return graph != null ? graph.Target : null; }
        }

        private static ProjectionStorage _field;
        public static ProjectionStorage PropA { get { return _field; } set { _field = value; } }
    }
}
