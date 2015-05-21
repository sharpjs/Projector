namespace Projector.ObjectModel
{
    // Projection (?) class has the private flags/value fields, and implements nested accessor subclasses.

    // However, when we reproject, we need shared values to be precached in the new projection.

    // What values are shared between projections?  --> Props of common bases.  To share disparate

    // Do NOT share props that have different behaviors, because behaviors affect the value.

    internal abstract class DirectPropertyAccessor<TProjection, TValue> : PropertyAccessor<TValue>
        where TProjection : Projection
    {
        protected readonly ProjectionProperty<TValue> property;

        protected DirectPropertyAccessor(ProjectionProperty<TValue> property)
        {
            this.property = property;
        }

        public sealed override ProjectionProperty2 Property
        {
            get { return property; }
        }

        private static TProjection Validate(Projection projection)
        {
            if (projection == null)
                throw Error.ArgumentNull("projection");

            return (TProjection) projection;
        }

        public sealed override bool GetValue(Projection projection, GetterOptions options, out TValue value)
        {
            var target = Validate(projection);

            if (TryGetCached(target, out value))
                return true;

            if (property.GetValue(target, options, out value))
                { Encache(target, value); return true; }
            else
                { /* Nothing to decache; */ return false; }
        }

        public sealed override bool SetValue(Projection projection, TValue value)
        {
            var target = Validate(projection);

            if (property.SetValue(target, value))
                { Encache(target, value); return true; }
            else
                { Decache(target); return false; }
        }

        public sealed override bool TryGetCached(Projection projection, out TValue value)
        {
            return TryGetCached(Validate(projection), out value);
        }

        public sealed override void Encache(Projection projection, TValue value)
        {
            Encache(Validate(projection), value);
        }

        public sealed override void Decache(Projection projection)
        {
            Decache(Validate(projection));
        }

        protected abstract bool TryGetCached (TProjection target, out TValue value);
        protected abstract void Encache      (TProjection target,     TValue value);
        protected abstract void Decache      (TProjection target);
    }
}
