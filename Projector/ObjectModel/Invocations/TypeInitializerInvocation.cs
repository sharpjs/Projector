﻿namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct TypeInitializerInvocation
    {
        private readonly ProjectionType            type;
        private readonly Cell<IProjectionBehavior> behavior;

        internal TypeInitializerInvocation(
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

                initializer.InitializeType
                (
                    new TypeInitializerInvocation
                        (type, behavior)
                );

                return;
            }
        }
    }
}
