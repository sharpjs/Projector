namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct PropertyClearInvocation<TValue>
    {
        private readonly Projection                      projection;
        private readonly ProjectionProperty              property;
        private readonly Cell<IPropertyBehavior<TValue>> behavior;

        internal PropertyClearInvocation(
            Projection                      projection,
            ProjectionProperty              property,
            Cell<IPropertyBehavior<TValue>> behavior)
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

        public void Proceed(TValue value)
        {
            var behavior = this.behavior;
            behavior.Item.ClearPropertyValue
            (
                new PropertyClearInvocation<TValue>
                    (projection, property, behavior.Next)
            );
        }
    }
}
