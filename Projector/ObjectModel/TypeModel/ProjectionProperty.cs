namespace Projector.ObjectModel
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using OverrideCollection = System.Collections.ObjectModel.ReadOnlyCollection<ProjectionProperty>;

    [DebuggerDisplay(@"\{{PropertyType.Name,nq} {Name,nq}\}")]
    public class ProjectionProperty : ProjectionMetaObject
    {
        private readonly string                   name;             // x
        private readonly ProjectionStructureType  declaringType;    // x
        private readonly ProjectionType           propertyType;     // x
        private readonly OverrideCollection       overrides;
        private readonly RuntimeMethodHandle      getterHandle;     // x
        private readonly RuntimeMethodHandle      setterHandle;     // x
        private readonly IPropertyAccessor[]      accessors;
        private ProjectionPropertyTraitAggregator aggregator;
        private Flags                             flags;            // x (read/write only)

        [Flags]
        private enum Flags
        {
            Declared  = 0x00000001, // Property is declared in the containing type (not inherited)
            CanRead   = 0x00000002, // Property has a getter
            CanWrite  = 0x00000004, // Property has a setter
            Reference = 0x00000008, // Property should participate in reference behavior
            Volatile  = 0x00000010, // Don't cache property's value (or 
            Default   = 0x00000020, // Property has a default value
        }

        internal ProjectionProperty(PropertyInfo property, ProjectionStructureType declaringType,
            ProjectionPropertyCollection properties, ProjectionFactory factory, ITraitResolution resolution)
        {
            this.name          = property.Name;
            this.declaringType = declaringType;
            this.propertyType  = factory.GetProjectionTypeUnsafe(property.PropertyType);
            this.accessors     = new IPropertyAccessor[4]; // factory.Providers.Count

            var getter = property.GetGetMethod();
            var setter = property.GetSetMethod();
            if (getter != null) { getterHandle = getter.MethodHandle; flags |= Flags.CanRead;  }
            if (setter != null) { setterHandle = setter.MethodHandle; flags |= Flags.CanWrite; }

            var aggregator = new ProjectionPropertyTraitAggregator(this, properties);
            resolution.ProvidePropertyTraits(this, property, aggregator);

            this.aggregator = aggregator;
            this.overrides  = aggregator.Overrides;
        }

        internal override void ComputeTraits()
        {
            var aggregator = this.aggregator;
            this.aggregator = null;
            aggregator.ComputeTraits();
        }

        internal override void InvokeInitializers()
        {
            new PropertyInitializerInvocation
                (this, Behaviors.First)
                .Proceed();
        }

        internal override void InvokePostInitializers()
        {
            new PropertyPostInitializerInvocation
                (this, Behaviors.First)
                .Proceed();
        }

        public string Name
        {
            get { return name; }
        }

        public ProjectionType ContainingType
        {
            get { return null; }
        }

        public ProjectionType DeclaringType
        {
            get { return (flags & Flags.Declared) != 0 ? declaringType : overrides[0].ContainingType; }
        }

        public ProjectionType PropertyType
        {
            get { return propertyType; }
        }

        public OverrideCollection Overrides
        {
            get { return overrides; }
        }

        /// <inheritdoc/>
        public sealed override ProjectionFactory Factory
        {
            get { return declaringType.Factory; }
        }

        public bool CanRead
        {
            get { return 0 != (flags & Flags.CanRead); }
        }

        public bool CanWrite
        {
            get { return 0 != (flags & Flags.CanWrite); }
        }

        //public bool IsShared
        //{
        //    get { return 0 != (flags & Flags.Shared); }
        //}

        //public bool IsReference
        //{
        //    get { return 0 != (flags & Flags.Reference); }
        //}

        public bool IsVolatile
        {
            get { return 0 != (flags & Flags.Volatile); }
        }

        public MethodInfo UnderlyingGetter
        {
            get { return CanRead ? GetMethod(getterHandle, declaringType) : null; }
        }

        public MethodInfo UnderlyingSetter
        {
            get { return CanWrite ? GetMethod(setterHandle, declaringType) : null; }
        }

        private static MethodInfo GetMethod(RuntimeMethodHandle handle, ProjectionType declaringType)
        {
            var typeHandle = declaringType.UnderlyingType.TypeHandle;
            return (MethodInfo) MethodBase.GetMethodFromHandle(handle, typeHandle);
        }

        internal IPropertyAccessor GetAccessor(int token)
        {
            return accessors[token];
        }

        internal void SetAccessor(int token, IPropertyAccessor accessor)
        {
            accessors[token] = accessor;
        }

        public virtual PropertyAccessor<T> As<T>()
        {
            return null; // TODO: make abstract
        }

        protected override void ApplyTraitBuilder(ITraitBuilder builder)
        {
            builder.ApplyPropertyTraits(new PropertyTraitApplicator(this));
        }

        public override string ToString()
        {
            return string.Concat(declaringType.ToString(), ".", name);
        }
    }
}
