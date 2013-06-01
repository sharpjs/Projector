namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct PropertyInitializerInvocation
    {
        private readonly ProjectionProperty        property;
        private readonly Cell<IProjectionBehavior> behavior;

        internal PropertyInitializerInvocation(
            ProjectionProperty        property,
            Cell<IProjectionBehavior> behavior)
        {
            this.property = property;
            this.behavior = behavior;
        }

        public ProjectionProperty Property
        {
            get { return property; }
        }

        public IEnumerable<object> Annotations
        {
            get { return property.Annotations; }
        }

        public IEnumerable<object> Behaviors
        {
            get { return property.Behaviors; }
        }

        public void Proceed()
        {
            for (var behavior = this.behavior;;)
            {
                if (behavior == null)
                    return;

                var initializer = behavior.Item as IPropertyInitializer;
                behavior        = behavior.Next;

                if (initializer != null)
                {
                    initializer.InitializeProperty
                    (
                        new PropertyInitializerInvocation
                            (property, behavior)
                    );
                    return;
                }
            }
        }
    }
}
