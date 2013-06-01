namespace Projector.ObjectModel
{
    public interface IPropertySetter : IProjectionBehavior
    {
        object SetPropertyValue(PropertySetterInvocation invocation, object value);
    }
}
