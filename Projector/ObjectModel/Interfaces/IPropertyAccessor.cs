namespace Projector.ObjectModel
{
    public interface IPropertyAccessor<T>
    {
        bool GetPropertyValue(
            object             store,
            Projection         projection,
            ProjectionProperty property,
            GetterOptions      options,
            out T              value);

        bool SetPropertyValue(
            object             store,
            Projection         projection,
            ProjectionProperty property,
            object             value);
    }

    public interface IPropertyAccessor
    {
        object GetPropertyValue(
            object             store,
            Projection         projection,
            ProjectionProperty property,
            GetterOptions  options);

        bool SetPropertyValue(
            object             store,
            Projection         projection,
            ProjectionProperty property,
            object             value);
    }
}
