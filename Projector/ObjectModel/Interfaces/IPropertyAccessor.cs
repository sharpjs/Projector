namespace Projector.ObjectModel
{
    public interface IPropertyAccessor
    {
        object GetPropertyValue(
            object             store,
            Projection         projection,
            ProjectionProperty property,
            GetterOptions  options);

        void SetPropertyValue(
            object             store,
            Projection         projection,
            ProjectionProperty property,
            object             value);
    }
}
