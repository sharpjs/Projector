//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using System.Text;

//namespace Projector.ObjectModel.TraitModel
//{
//    internal class CacheBehavior<TTarget, TValue> : IPropertyBehavior<TValue>, IMixin
//    {
//        public void ModifyImplementation(TypeBuilder implementationType)
//        {
//            throw new NotImplementedException();
//        }

//        public void InitializeImplementation(Type implementationType)
//        {
//            throw new NotImplementedException();
//        }

//        public void ModifyImplementation(TypeBuilder implementationType, ProjectionProperty property)
//        {
//            throw new NotImplementedException();
//        }

//        public void InitializeImplementation(Type implementationType, ProjectionProperty property)
//        {
//            throw new NotImplementedException();
//        }

//        private CacheAccessor<TValue> cache;

//        public bool GetPropertyValue(PropertyGetterInvocation<TValue> invocation, out TValue value)
//        {
//            if (cache.TryGetValue(invocation.Projection, out value))
//                return true;

//            if (invocation.Proceed(out value))
//                { cache.SetValue(invocation.Projection, value); return true; }
//            else
//                { /* Nothing to decache; */ return false; }
//        }

//        public bool SetPropertyValue(PropertySetterInvocation<TValue> invocation, TValue value)
//        {
//        }
//    }

//    internal abstract class CacheAccessor<T>
//    {
//        public abstract bool TryGetValue(Projection projection, out T value);
//        public abstract bool    SetValue(Projection projection,     T value);
//        public abstract void  ClearValue(Projection projection);
//    }
//}
