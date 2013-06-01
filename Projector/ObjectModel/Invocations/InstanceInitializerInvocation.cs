namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct InstanceInitializerInvocation
    {
        private readonly Projection                projection;
        private readonly object                    arguments;
        private readonly Cell<IProjectionBehavior> behavior;

        internal InstanceInitializerInvocation(
            Projection                projection,
            object                    arguments,
            Cell<IProjectionBehavior> behavior)
        {
            this.projection = projection;
            this.arguments  = arguments;
            this.behavior   = behavior;
        }

        public Projection Projection
        {
            get { return projection; }
        }

        public ProjectionInstance Instance
        {
            get { return projection.Instance; }
        }

        public object Context
        {
            get { return projection.Instance.Context; }
        }

        public T GetArgument<T>()
            where T : class
        {
            T candidate;
            object[] array;

            if (arguments == null)
                return null;

            if (null != (candidate = arguments as T))
                return candidate;

            if (null != (array = arguments as object[]))
                for (var i = 0; i < array.Length; i++)
                    if (null != (candidate = array[i] as T))
                        return candidate;

            return null;
        }

        public void Proceed()
        {
            for (var behavior = this.behavior;;)
            {
                if (behavior == null)
                    return;

                var initializer = behavior.Item as IInstanceInitializer;
                behavior        = behavior.Next;

                if (initializer != null)
                {
                    initializer.InitializeInstance
                    (
                        new InstanceInitializerInvocation
                            (projection, arguments, behavior)
                    );
                    return;
                }
            }
        }
    }
}
