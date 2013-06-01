namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct ProjectionInitializerInvocation
    {
        private readonly Projection                projection;
        private readonly Cell<IProjectionBehavior> behavior;

        internal ProjectionInitializerInvocation(
            Projection                projection,
            Cell<IProjectionBehavior> behavior)
        {
            this.projection = projection;
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

        public void Proceed()
        {
            for (var behavior = this.behavior;;)
            {
                if (behavior == null)
                    return;

                var initializer = behavior.Item as IProjectionInitializer;
                behavior        = behavior.Next;

                if (initializer != null)
                {
                    initializer.InitializeProjection
                    (
                        new ProjectionInitializerInvocation
                            (projection, behavior)
                    );
                    return;
                }
            }
        }
    }
}
