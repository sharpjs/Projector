namespace Projector.ObjectModel
{
    internal sealed class ConvertingPropertyAccessor<TOuter, TInner> : PropertyAccessor<TOuter>
    {
        private readonly PropertyAccessor<TInner>   accessor;
        private readonly IConverter<TOuter, TInner> converter;

        internal ConvertingPropertyAccessor(PropertyAccessor<TInner> accessor, IConverter<TOuter, TInner> converter)
        {
            if (accessor == null)
                throw Error.ArgumentNull("accessor");
            if (converter == null)
                throw Error.ArgumentNull("converter");

            this.accessor  = accessor;
            this.converter = converter;
        }

        public override ProjectionProperty2 Property
        {
            get { return accessor.Property; }
        }

        public override bool GetValue(Projection projection, GetterOptions options, out TOuter value)
        {
            TInner inner;
            var result = accessor.GetValue(projection, options, out inner);
            value = converter.ToOuter(inner);
            return result;
        }

        public override bool SetValue(Projection projection, TOuter value)
        {
            return accessor.SetValue(projection, converter.ToInner(value));
        }

        public override bool TryGetCached(Projection projection, out TOuter value)
        {
            TInner inner;
            var result = accessor.TryGetCached(projection, out inner);
            value = converter.ToOuter(inner);
            return result;
        }

        public override void Encache(Projection projection, TOuter value)
        {
            accessor.Encache(projection, converter.ToInner(value));
        }

        public override void Decache(Projection projection)
        {
            accessor.Decache(projection);
        }
    }
}
