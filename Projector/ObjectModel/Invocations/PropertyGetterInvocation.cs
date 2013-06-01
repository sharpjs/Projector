namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct PropertyGetterInvocation
    {
        private readonly Projection                projection;
        private readonly ProjectionProperty        property;
        private readonly InvocationOptions         options;
        private readonly Cell<IProjectionBehavior> behavior;

        internal PropertyGetterInvocation(
            Projection                projection,
            ProjectionProperty        property,
            InvocationOptions         options,
            Cell<IProjectionBehavior> behavior)
        {
            this.projection = projection;
            this.property   = property;
            this.options    = options;
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

        public InvocationOptions Options
        {
            get { return options; }
        }

        public object Proceed()
        {
            for (var behavior = this.behavior;;)
            {
                if (behavior == null)
                    return Unknown.Value;

                var getter = behavior.Item as IPropertyGetter;
                behavior   = behavior.Next;

                if (getter != null)
                {
                    return getter.GetPropertyValue
                    (
                        new PropertyGetterInvocation
                            (projection, property, options, behavior)
                    );
                }
            }
        }
    }
}
