namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct PropertyGetterInvocation
    {
        private readonly Projection                projection;
        private readonly ProjectionProperty        property;
        private readonly GetterOptions         options;
        private readonly Cell<IProjectionBehavior> behavior;

        internal PropertyGetterInvocation(
            Projection                projection,
            ProjectionProperty        property,
            GetterOptions         options,
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

        public GetterOptions Options
        {
            get { return options; }
        }

        public object Proceed()
        {
            for (var behavior = this.behavior;;)
            {
                if (behavior == null)
                    return AccessStorage();

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

        private object AccessStorage()
        {
            for (var cell = projection.Instance.FirstStorage; cell != null; cell = cell.Next)
            {
                var item = cell.Item;

                var accessor = property.GetAccessor(item.Token);
                if (accessor == null)
                    continue;

                var value = accessor.GetPropertyValue(item.Store, projection, property, options);
                if (value is Unknown)
                    continue;

                return value;
            }

            return Unknown.Value;
        }
    }
}
