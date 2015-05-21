﻿namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    public struct PropertySetterInvocation<TValue>
    {
        private readonly Projection                      projection;
        private readonly ProjectionProperty              property;
        private readonly Cell<IPropertyBehavior<TValue>> next;

        internal PropertySetterInvocation(
            Projection                      projection,
            ProjectionProperty              property,
            Cell<IPropertyBehavior<TValue>> next)
        {
            this.projection = projection;
            this.property   = property;
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

        public bool Proceed(TValue value)
        {
            return next.Item.SetPropertyValue
            (
                new PropertySetterInvocation<TValue>
                    (projection, property, next.Next),
                value
            );
        }
    }
}
