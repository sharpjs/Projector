//namespace Projector.ObjectModel
//{
//    using System;

//    internal class StorageBehavior<T> : IPropertyBehavior<T>
//    {
//        public bool GetPropertyValue(PropertyGetterInvocation<T> invocation, out T value)
//        {
//            value = default(T); // XXX

//            for (var cell = invocation.Projection.Instance.FirstStorage; cell != null; cell = cell.Next)
//            {
//                var storage = cell.Item;

//                var accessor = invocation.Property.GetAccessor(storage.Token);
//                if (accessor == null)
//                    continue;

//                var obj = accessor.GetPropertyValue(storage.Store, invocation.Projection, invocation.Property, invocation.Options);
//                if (obj is Unknown)
//                    continue;

//                value = (T) obj; // XXX
//            }

//            return true; // XXX
//        }

//        public bool SetPropertyValue(PropertySetterInvocation<T> invocation, T value)
//        {
//            for (var cell = invocation.Projection.Instance.FirstStorage; cell != null; cell = cell.Next)
//            {
//                var storage = cell.Item;

//                var accessor = invocation.Property.GetAccessor(storage.Token);
//                if (accessor == null)
//                    continue;

//                accessor.SetPropertyValue(storage.Store, invocation.Projection, invocation.Property, value);
//            }

//            return true; // XXX
//        }
//    }
//}
