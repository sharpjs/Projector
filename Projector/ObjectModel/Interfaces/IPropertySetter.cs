namespace Projector.ObjectModel
{
    public interface IPropertySetter<in TIn, TOut> : IProjectionBehavior
    {
        bool GetPropertyValue(PropertyGetterInvocation<TOut> invocation, TIn value);
    }

    public interface IPropertySetter : IProjectionBehavior
    {
        bool SetPropertyValue(PropertySetterInvocation invocation, object value);
        // NOTE: Return true  => value can be cached
        //              false => different value will be retrieved; don't cache
    }
}
