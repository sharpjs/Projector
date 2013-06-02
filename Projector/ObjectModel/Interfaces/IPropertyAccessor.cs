namespace Projector.ObjectModel
{
    public interface IPropertyAccessor
    {
        object GetPropertyValue(
            object             store,
            Projection         projection,
            ProjectionProperty property,
            InvocationOptions  options);

        void SetPropertyValue(
            object             store,
            Projection         projection,
            ProjectionProperty property,
            object             value);
    }
}
