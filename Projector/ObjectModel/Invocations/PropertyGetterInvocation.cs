namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct PropertyGetterInvocation<TValue>
    {
        private readonly Projection                      projection;
        private readonly ProjectionProperty              property;
        private readonly GetterOptions                   options;
        private readonly Cell<IPropertyBehavior<TValue>> next;

        internal PropertyGetterInvocation(
            Projection                      projection,
            ProjectionProperty              property,
            GetterOptions                   options,
            Cell<IPropertyBehavior<TValue>> next)
        {
            this.projection = projection;
            this.property   = property;
            this.options    = options;
            this.next       = next;
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

        public GetterOptions Options
        {
            get { return options; }
        }

        public bool Proceed(out TValue value)
        {
            return next.Item.GetPropertyValue
            (
                new PropertyGetterInvocation<TValue>
                    (projection, property, options, next.Next),
                out value
            );
        }
    }
}
