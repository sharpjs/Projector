namespace Projector.ObjectModel
{
    using System;
    using SCM = System.ComponentModel;
    using System.Diagnostics;

    [DebuggerDisplay("Projection: {Type.Name,nq}")]
    public abstract class Projection : ProjectionObject
    {
        private readonly ProjectionInstance instance;
        //  private readonly States             state;

        //[Flags]
        //private enum States
        //{
        //	CanEdit     = 0x00000001,
        //	CanNotify   = 0x00000002,
        //	CanValidate = 0x00000004
        //}

        protected Projection(ProjectionInstance instance)
        {
            this.instance = instance;
            //	this.state    = GetState();
        }

        public abstract ProjectionType Type
        {
            get; // Implemented by dynamic subclass
        }

        public ProjectionInstance Instance
        {
            get { return instance; }
        }

        /// <inheritdoc/>
        public sealed override ProjectionFactory Factory
        {
            get { return instance.Factory; }
        }

        //private States GetState()
        //{
        //	var state = default(States);
        //	if (typeof(SCM.IEditableObject)       .IsAssignableFrom(Type.UnderlyingType))
        //		state |= States.CanEdit;
        //	if (typeof(SCM.INotifyPropertyChanged).IsAssignableFrom(Type.UnderlyingType))
        //		state |= States.CanNotify;
        //	if (typeof(SCM.IDataErrorInfo)        .IsAssignableFrom(Type.UnderlyingType))
        //		state |= States.CanValidate;
        //	return state;
        //}

        public object GetPropertyValue(ProjectionProperty property, GetterOptions options)
        {
            var site = Type.Properties.GetSite(property);

            return GetPropertyValueCore(Type.Properties[property], options);
        }

        protected object GetPropertyValueCore(ProjectionProperty property, GetterOptions options)
        {
            return new PropertyGetterInvocation
                (this, property, options, property.FirstBehavior)
                .Proceed();
        }

        public bool SetPropertyValue(ProjectionProperty property, object value)
        {
            return SetPropertyValueCore(Type.Properties[property], value);
        }

        protected bool SetPropertyValueCore(ProjectionProperty property, object value)
        {
            return new PropertySetterInvocation
                (this, property, property.FirstBehavior)
                .Proceed(value);
        }

        public T GetPropertyValueAs<T>(ProjectionProperty property, GetterOptions options)
        {
            return (T) GetPropertyValue(property, options);
        }

        public bool SetPropertyValueAs<T>(ProjectionProperty property, T value)
        {
            return SetPropertyValue(property, value);
        }

        public void InvalidateProperty(ProjectionProperty property)
        {
            var site = Type.Properties.GetSite(property);
        }
    }
}
