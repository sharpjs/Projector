namespace Projector.ObjectModel
{
    public interface IPropertyGetter<TIn, TOut> : IProjectionBehavior
    {
        bool GetPropertyValue(PropertyGetterInvocation<TIn> invocation, out TOut value);
    }

    public interface IPropertyGetter : IProjectionBehavior
    {
        object GetPropertyValue(PropertyGetterInvocation invocation);
    }
}
