using Projector.ObjectModel.TypeModel;
namespace Projector.ObjectModel
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using OverrideCollection = System.Collections.ObjectModel.ReadOnlyCollection<ProjectionProperty>;

    [DebuggerDisplay(@"\{{PropertyType.Name,nq} {Name,nq}\}")]
    public abstract class ProjectionProperty2 : ProjectionMetaObject
    {
        private readonly ProjectionStructureType containingType;
        private readonly int                     index;

        private readonly IPropertyAccessor[]      accessors;
        private Flags                             flags;

        [Flags]
        private enum Flags
        {
            CanRead   = 0x00000001,
            CanWrite  = 0x00000002,
            Shared    = 0x00000004,
            Reference = 0x00000008,
            Volatile  = 0x00000010,
            Eager     = 0x00000020,
            Default   = 0x00000100,
        }

        internal ProjectionProperty2(ProjectionStructureType containingType, int index)
        {
            this.containingType = containingType;
            this.index          = index;
            this.accessors     = new IPropertyAccessor[4]; // factory.Providers.Count
        }

        //internal ProjectionProperty(PropertyInfo property, ProjectionStructureType declaringType,
        //    ProjectionPropertyCollection properties, ProjectionFactory factory, ITraitResolution resolution)
        //{
        //    this.name          = property.Name;
        //    this.declaringType = declaringType;
        //    this.propertyType  = factory.GetProjectionTypeUnsafe(property.PropertyType);
        //    this.accessors     = new IPropertyAccessor[4]; // factory.Providers.Count

        //    var getter = property.GetGetMethod();
        //    var setter = property.GetSetMethod();
        //    if (getter != null) { getterHandle = getter.MethodHandle; flags |= Flags.CanRead;  }
        //    if (setter != null) { setterHandle = setter.MethodHandle; flags |= Flags.CanWrite; }

        //    var aggregator = new ProjectionPropertyTraitAggregator(this, properties);
        //    resolution.ProvidePropertyTraits(this, property, aggregator);

        //    this.aggregator = aggregator;
        //    this.overrides  = aggregator.Overrides;
        //}

        //protected override void ApplyTraitBuilder(ITraitBuilder builder)
        //{
        //    builder.ApplyPropertyTraits(new PropertyTraitApplicator(this));
        //}

        //internal override TraitAggregator CreateTraitAggregator()
        //{
        //    var aggregator = this.aggregator;
        //    this.aggregator = null;
        //    return aggregator;
        //}

        //internal override void InvokeInitializers()
        //{
        //    //new ProjectionPropertyInitializerInvocation
        //    //    (this, FirstBehavior)
        //    //    .Proceed();
        //}

        //internal override void InvokeLateInitializers()
        //{
        //    //new ProjectionPropertyLateInitializerInvocation
        //    //    (this, FirstBehavior)
        //    //    .Proceed();
        //}

        public abstract string Name
        {
            get;
        }

        public ProjectionType ContainingType
        {
            get { return containingType; }  
        }

        public abstract ProjectionType DeclaringType
        {
            get;
        }

        //public ProjectionType PropertyType
        //{
        //    get { return propertyType; }
        //}

        //public OverrideCollection Overrides
        //{
        //    get { return overrides; }
        //}

        ///// <inheritdoc/>
        //public sealed override ProjectionFactory Factory
        //{
        //    get { return declaringType.Factory; }
        //}

        //public bool CanRead
        //{
        //    get { return 0 != (flags & Flags.CanRead); }
        //}

        //public bool CanWrite
        //{
        //    get { return 0 != (flags & Flags.CanWrite); }
        //}

        ////public bool IsShared
        ////{
        ////    get { return 0 != (flags & Flags.Shared); }
        ////}

        ////public bool IsReference
        ////{
        ////    get { return 0 != (flags & Flags.Reference); }
        ////}

        ////public bool IsVolatile
        ////{
        ////    get { return 0 != (flags & Flags.Volatile); }
        ////}

        //public MethodInfo UnderlyingGetter
        //{
        //    get { return CanRead ? GetMethod(getterHandle, declaringType) : null; }
        //}

        //public MethodInfo UnderlyingSetter
        //{
        //    get { return CanWrite ? GetMethod(setterHandle, declaringType) : null; }
        //}

        //private static MethodInfo GetMethod(RuntimeMethodHandle handle, ProjectionType declaringType)
        //{
        //    var typeHandle = declaringType.UnderlyingType.TypeHandle;
        //    return (MethodInfo) MethodBase.GetMethodFromHandle(handle, typeHandle);
        //}

        //internal IPropertyAccessor GetAccessor(int token)
        //{
        //    return accessors[token];
        //}

        //internal void SetAccessor(int token, IPropertyAccessor accessor)
        //{
        //    accessors[token] = accessor;
        //}

        //public virtual PropertyAccessor<T> As<T>()
        //{
        //    return null; // TODO: make abstract
        //}

        //public override string ToString()
        //{
        //    return string.Concat(declaringType.ToString(), ".", name);
        //}

        internal override void InvokeInitializers()
        {
            //new PropertyInitializerInvocation
            //    (this, Behaviors.First)
            //    .Proceed();
        }

        internal override void InvokePostInitializers()
        {
            //new PropertyPostInitializerInvocation
            //    (this, Behaviors.First)
            //    .Proceed();
        }

        internal abstract void SetAccessor(object accessor);

        public override ProjectionFactory Factory
        {
            get { throw new NotImplementedException(); }
        }
    }

    [DebuggerDisplay(@"\{{PropertyType.Name,nq} {Name,nq}\}")]
    internal abstract class ProjectionProperty<T> : ProjectionProperty2
    {
        private readonly BehaviorSet<IPropertyBehavior<T>> behaviors;
        private          PropertyAccessor<T>               accessor;

        internal ProjectionProperty(ProjectionStructureType containingType, int index)
            : base(containingType, index)
        {
            behaviors = new BehaviorSet<IPropertyBehavior<T>>();
        }

        //public override PropertyAccessor<TOther> As<TOther>()
        //{
        //    var accessor = (PropertyAccessor<T>) Activator.CreateInstance
        //    (
        //        typeof(DirectPropertyAccessor<,>)
        //            .MakeGenericType(DeclaringType, PropertyType)
        //    );

        //    return accessor.As<TOther>();
        //}

        protected override void ApplyBehavior(IProjectionBehavior behavior)
        {
            ValidateBehavior(behavior);
            base.ApplyBehavior(behavior);
        }

        private void ValidateBehavior(IProjectionBehavior behavior)
        {
            if (behavior is IPropertyBehavior<T>)
                return;

            foreach (var type in behavior.GetType().GetInterfaces())
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPropertyBehavior<>))
                    throw Error.TodoError(); // Attempted to apply property behavior of wrong type
        }

        internal bool GetValue(Projection projection, GetterOptions options, out T value)
        {
            return new PropertyGetterInvocation<T>
                (projection, null/*this*/, options, behaviors.First)
                .Proceed(out value);
        }

        internal bool SetValue(Projection projection, T value)
        {
            return new PropertySetterInvocation<T>
                (projection, null/*this*/, behaviors.First)
                .Proceed(value);
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override ProjectionType DeclaringType
        {
            get { throw new NotImplementedException(); }
        }

        internal override void ComputeTraits()
        {
            throw new NotImplementedException();
        }

        protected override void ApplyTraitBuilder(ITraitBuilder builder)
        {
            throw new NotImplementedException();
        }

        internal override void SetAccessor(object accessor)
        {
            accessor = (PropertyAccessor<T>) accessor;
        }
    }

    internal sealed class ProjectionInheritedProperty<T> : ProjectionProperty<T>
    {
        private readonly ProjectionDeclaredProperty<T> declaredProperty;

        public ProjectionInheritedProperty(ProjectionStructureType containingType, int index, ProjectionDeclaredProperty<T> parent)
            : base(containingType, index)
        {
            this.declaredProperty = parent;
        }

        public override string Name
        {
            get { return declaredProperty.Name; }
        }

        public override ProjectionType DeclaringType
        {
            get { return declaredProperty.ContainingType; }
        }

        internal override void ComputeTraits()
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ProjectionDeclaredProperty<T> : ProjectionProperty<T>
    {
        private readonly string              name;
        private readonly ProjectionType      propertyType;
        private readonly OverrideCollection  overrides;
        private readonly RuntimeMethodHandle getterHandle;
        private readonly RuntimeMethodHandle setterHandle;

        // Data used only during initialization is kept in this field.
        // After initialiation, this field becomes null so that data can be reclaimed by GC.
        private ProjectionDeclaredPropertyInitialization<T> aggregator;

        public ProjectionDeclaredProperty(ProjectionStructureType containingType, int index, PropertyInfo property,
            ProjectionPropertyCollection properties, ProjectionFactory factory, ITraitResolution resolution)
            : base(containingType, index)
        {
            this.name          = property.Name;
            this.propertyType  = factory.GetProjectionTypeUnsafe(property.PropertyType);

            var getter = property.GetGetMethod();
            var setter = property.GetSetMethod();
            if (getter != null) { getterHandle = getter.MethodHandle; /* flags |= Flags.CanRead; */ }
            if (setter != null) { setterHandle = setter.MethodHandle; /* flags |= Flags.CanWrite;*/ }

            var aggregator = new ProjectionDeclaredPropertyInitialization<T>(this, properties);
            resolution.ProvidePropertyTraits(this as ProjectionProperty /*XXX*/, property, aggregator);

            this.aggregator = aggregator;
            this.overrides  = aggregator.Overrides;
        }

        public override string Name
        {
            get { return name; }
        }

        public override ProjectionType DeclaringType
        {
            get { return ContainingType; }
        }

        internal override void ComputeTraits()
        {
            throw new NotImplementedException();
        }
    }
}
