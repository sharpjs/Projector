namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct TypePostInitializerInvocation
    {
        private readonly ProjectionType            type;
        private readonly Cell<IProjectionBehavior> behavior;

        internal TypePostInitializerInvocation(
            ProjectionType            type,
            Cell<IProjectionBehavior> behavior)
        {
            this.type     = type;
            this.behavior = behavior;
        }

        public ProjectionType Type
        {
            get { return type; }
        }

        public void Proceed()
        {
            for (var behavior = this.behavior;;)
            {
                if (behavior == null)
                    return;

                var initializer = behavior.Item as ITypeInitializer;
                behavior        = behavior.Next;

                if (initializer == null)
                    continue;

                initializer.PostInitializeType
                (
                    new TypePostInitializerInvocation
                        (type, behavior)
                );

                return;
            }
        }
    }
}
