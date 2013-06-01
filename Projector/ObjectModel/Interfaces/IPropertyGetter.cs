namespace Projector.ObjectModel
{
    public interface IPropertyGetter : IProjectionBehavior
    {
        object GetPropertyValue(PropertyGetterInvocation invocation);
    }
}
