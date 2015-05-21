using System;

namespace Projector.ObjectModel
{
    internal sealed class VolatilePropertyAccessor<TProjection, TValue> : DirectPropertyAccessor<TProjection, TValue>
        where TProjection : Projection
    {
        internal VolatilePropertyAccessor(ProjectionProperty<TValue> property)
            : base(property) { }

        protected override bool TryGetCached(TProjection target, out TValue value)
        {
            // Volatile properties are not cached
            value = default(TValue);
            return false;
        }

        protected override void Encache(TProjection target, TValue value)
        {
            // Volatile properties are not cached
        }

        protected override void Decache(TProjection target)
        {
            // Volatile properties are not cached
        }
    }
}
