namespace Projector.ObjectModel
{
    /// <summary>
    ///   Provides methods to get, set, and cache the value of a projection property.
    /// </summary>
    /// <typeparam name="T">
    ///   The type of the property value.
    /// </typeparam>
    public abstract class PropertyAccessor<T>
    {
        protected PropertyAccessor() { }

        public abstract ProjectionProperty2 Property { get; }

        public PropertyAccessor<TOther> As<TOther>()
        {
            if (typeof(TOther) == typeof(T))
                return this as PropertyAccessor<TOther>;

            var converter = ConverterRegistry.GetConverter<TOther, T>(); // TODO: get registry
            return new ConvertingPropertyAccessor<TOther, T>(this, converter);
        }

        public T GetValue(Projection projection)
        {
            T value;
            GetValue(projection, GetterOptions.Virtual, out value);
            return value;
        }

        public T GetValue(Projection projection, GetterOptions options)
        {
            T value;
            GetValue(projection, options, out value);
            return value;
        }

        public bool GetValue(Projection projection, out T value)
        {
            return GetValue(projection, GetterOptions.Virtual, out value);
        }

        public abstract bool GetValue(Projection projection, GetterOptions options, out T value);

        public abstract bool SetValue(Projection projection, T value);

        public abstract bool TryGetCached(Projection projection, out T value);
        public abstract void Encache     (Projection projection,     T value);
        public abstract void Decache     (Projection projection);
    }
}
