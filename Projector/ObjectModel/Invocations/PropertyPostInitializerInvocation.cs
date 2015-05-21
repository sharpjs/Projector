namespace Projector.ObjectModel
{
    public struct PropertyPostInitializerInvocation
    {
        private readonly ProjectionProperty        property;
        private readonly Cell<IProjectionBehavior> behavior;

        internal PropertyPostInitializerInvocation(
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

        public void Proceed()
        {
            for (var behavior = this.behavior;;)
            {
                if (behavior == null)
                    return;

                var initializer = behavior.Item as IPropertyInitializer;
                behavior        = behavior.Next;

                if (initializer == null)
                    continue;

                initializer.PostInitializeProperty
                (
                    new PropertyPostInitializerInvocation
                        (property, behavior)
                );

                return;
            }
        }
    }
}
