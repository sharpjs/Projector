//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Projector.ObjectModel
//{
//    internal abstract class PropertyBehaviorNode<TValue>
//    {
//        protected PropertyBehaviorNode() { }

//        internal abstract bool InvokeGet(Projection projection, ProjectionProperty property, GetterOptions options, out TValue value);
//        internal abstract bool InvokeSet(Projection projection, ProjectionProperty property,                            TValue value);
//    }

//    internal sealed class PropertyBehaviorNode<TValue, TUnderlying> : PropertyBehaviorNode<TValue>
//    {
//        private readonly IPropertyBehavior<TValue, TUnderlying> behavior;
//        private readonly PropertyBehaviorNode<TUnderlying>      next;

//        internal PropertyBehaviorNode(IPropertyBehavior<TValue, TUnderlying> behavior, PropertyBehaviorNode<TUnderlying> next)
//        {
//            this.behavior = behavior;
//            this.next     = next;
//        }

//        internal override bool InvokeGet(Projection projection, ProjectionProperty property, GetterOptions options, out TValue value)
//        {
//            return behavior.GetPropertyValue
//            (
//                new PropertyGetterInvocation<TUnderlying>(projection, property, options, next),
//                out value
//            );
//        }

//        internal override bool InvokeSet(Projection projection, ProjectionProperty property, TValue value)
//        {
//            return behavior.SetPropertyValue
//            (
//                new PropertySetterInvocation<TUnderlying>(projection, property, next),
//                value
//            );
//        }
//    }
//}
