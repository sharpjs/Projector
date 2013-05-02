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
        //private readonly Flags                    flags;
        private readonly ProjectionStructureType  declaringType;
        private readonly ProjectionType           propertyType;
        private readonly RuntimeMethodHandle      getterHandle;
        private readonly RuntimeMethodHandle      setterHandle;
        private readonly OverrideCollection       overrides;
        private ProjectionPropertyTraitAggregator aggregator;

        //[Flags]
        //private enum Flags
        //{
        //    CanRead   = 0x00000001,
        //    CanWrite  = 0x00000002,
        //    Shared    = 0x00000004,
        //    Reference = 0x00000008,
        //    Volatile  = 0x00000010,
        //    Eager     = 0x00000020,
        //    Default   = 0x00000100,
        //}

        internal ProjectionProperty(PropertyInfo property, ProjectionStructureType declaringType,
            ProjectionPropertyCollection properties, ProjectionFactory factory)
            //: base(factory)
        {
            this.name           = property.Name;
            this.declaringType  = declaringType;
            this.propertyType   = factory.GetProjectionTypeUnsafe(property.PropertyType);
            this.getterHandle   = property.GetGetMethod().MethodHandle;
            this.setterHandle   = property.GetSetMethod().MethodHandle;

            //if (property.CanRead ) flags |= Flags.CanRead;
            //if (property.CanWrite) flags |= Flags.CanWrite;

            aggregator = new ProjectionPropertyTraitAggregator(this, property, properties);
            this.overrides = aggregator.CollectOverrides();
        }

        internal override TraitAggregator CreateTraitAggregator()
        {
            var value = aggregator;
            aggregator = null;
            return value;
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

        //public bool CanRead
        //{
        //    get { return 0 != (flags & Flags.CanRead); }
        //}

        //public bool CanWrite
        //{
        //    get { return 0 != (flags & Flags.CanWrite); }
        //}

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

        public override string ToString()
        {
            return string.Concat(declaringType.ToString(), ".", name);
        }

        public MethodBase UnderlyingGetter
        {
            get { return getterHandle != null ? MethodBase.GetMethodFromHandle(getterHandle) : null; }
        }

        public MethodBase UnderlyingSetter
        {
            get { return setterHandle != null ? MethodBase.GetMethodFromHandle(setterHandle) : null; }
        }

        public PropertyInfo GetUnderlyingProperty()
        {
            return declaringType.UnderlyingType.GetProperty(name, PublicDeclaredBinding);
        }

        private BindingFlags PublicDeclaredBinding
             = BindingFlags.Instance
             | BindingFlags.Public
             | BindingFlags.DeclaredOnly;
    }
}
