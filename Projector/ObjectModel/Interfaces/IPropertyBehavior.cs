using System;

namespace Projector.ObjectModel
{
    public interface IPropertyBehavior<TValue> : IProjectionBehavior
    {
        bool GetPropertyValue(PropertyGetterInvocation<TValue> invocation, out TValue value);
        // true => value is present in underlying storage mechanism

        bool SetPropertyValue(PropertySetterInvocation<TValue> invocation, TValue value);
        // true => value has been stored directly, not copied; no need to get it again after this

        void ClearPropertyValue(PropertyClearInvocation<TValue> invocation);
        // always have to do a get again after this
    }
}
