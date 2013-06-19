namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct PropertySetterInvocation
    {
        private readonly Projection                projection;
        private readonly ProjectionProperty        property;
        private readonly Cell<IProjectionBehavior> behavior;

        internal PropertySetterInvocation(
            Projection                projection,
            ProjectionProperty        property,
            Cell<IProjectionBehavior> behavior)
        {
            this.projection = projection;
            this.property   = property;
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

        public bool Proceed(object value)
        {
            for (var behavior = this.behavior;;)
            {
                if (behavior == null)
                    return AccessStorage(value);

                var setter = behavior.Item as IPropertySetter;
                behavior   = behavior.Next;

                if (setter != null)
                {
                    return setter.SetPropertyValue
                    (
                        new PropertySetterInvocation
                            (projection, property, behavior),
                        value
                    );
                }
            }
        }

        private bool AccessStorage(object value)
        {
            var cacheable = true;

            for (var cell = projection.Instance.FirstStorage; cell != null; cell = cell.Next)
            {
                var item = cell.Item;

                var accessor = property.GetAccessor(item.Token);
                if (accessor == null)
                    continue;

                if (!accessor.SetPropertyValue(item.Store, projection, property, value))
                    cacheable = false;
            }

            return cacheable;
        }
    }
}
