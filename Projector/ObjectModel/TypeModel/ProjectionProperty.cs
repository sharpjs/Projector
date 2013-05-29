namespace Projector.ObjectModel
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using OverrideCollection = System.Collections.ObjectModel.ReadOnlyCollection<ProjectionProperty>;

    [DebuggerDisplay(@"\{{PropertyType.Name,nq} {Name,nq}\}")]
    public sealed class ProjectionProperty : ProjectionMetaObject
    {
        private readonly string                   name;
        private readonly ProjectionStructureType  declaringType;
        private readonly ProjectionType           propertyType;
        private readonly RuntimeMethodHandle      getterHandle;
        private readonly RuntimeMethodHandle      setterHandle;
        private readonly OverrideCollection       overrides;
        private ProjectionPropertyTraitAggregator aggregator;
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

        internal ProjectionProperty(PropertyInfo property, ProjectionStructureType declaringType,
            ProjectionPropertyCollection properties, ProjectionFactory factory, ITraitResolution resolution)
            //: base(factory)
        {
            this.name          = property.Name;
            this.declaringType = declaringType;
            this.propertyType  = factory.GetProjectionTypeUnsafe(property.PropertyType);

            var getter = property.GetGetMethod();
            var setter = property.GetSetMethod();
            if (getter != null) { getterHandle = getter.MethodHandle; flags |= Flags.CanRead;  }
            if (setter != null) { setterHandle = setter.MethodHandle; flags |= Flags.CanWrite; }

            var aggregator = new ProjectionPropertyTraitAggregator(this, properties);
            resolution.ProvidePropertyTraits(this, property, aggregator);

            this.aggregator = aggregator;
            this.overrides  = aggregator.Overrides;
        }

        internal override TraitAggregator CreateTraitAggregator()
        {
            var aggregator = this.aggregator;
            this.aggregator = null;
            return aggregator;
        }

        internal override void InvokeInitializers()
        {
            //new ProjectionPropertyInitializerInvocation
            //    (this, FirstBehavior)
            //    .Proceed();
        }

        internal override void InvokeLateInitializers()
        {
            //new ProjectionPropertyLateInitializerInvocation
            //    (this, FirstBehavior)
            //    .Proceed();
        }

        public string Name
        {
            get { return name; }
        }

        public ProjectionType DeclaringType
        {
            get { return declaringType; }
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

        //public bool IsVolatile
        //{
        //    get { return 0 != (flags & Flags.Volatile); }
        //}

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

        public override string ToString()
        {
            return string.Concat(declaringType.ToString(), ".", name);
        }
    }
}
