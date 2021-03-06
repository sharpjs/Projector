﻿namespace Projector.ObjectModel
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay(@"\{Name = {Name}, FullName = {FullName}\}")]
    public abstract class ProjectionType : ProjectionMetaObject
    {
        private readonly Type              type;
        private readonly TypeKind          kind;
        private readonly ProjectionFactory factory;
        private          ITraitResolution  resolution;

        internal ProjectionType(Type type, TypeKind kind, ProjectionFactory factory)
            //: base(factory)
        {
            this.type    = type;
            this.kind    = kind;
            this.factory = factory;
        }

        internal override TraitAggregator CreateTraitAggregator()
        {
            var resolution = this.resolution ?? ResolveTraits();
            this.resolution = null;
            return new ProjectionTypeTraitAggregator(this, resolution);
        }

        internal override void InvokeInitializers()
        {
            new TypeInitializerInvocation
                (this, FirstBehavior)
                .Proceed();
        }

        internal override void InvokeLateInitializers()
        {
            //new TypeLateInitializerInvocation
            //    (this, FirstBehavior)
            //    .Proceed();
        }

        /// <summary>
        ///   Gets the general kind of the type.
        /// </summary>
        public TypeKind Kind
        {
            get { return kind; }
        }

        /// <summary>
        ///   Gets the underlying <c>System.Type</c> of the type.
        /// </summary>
        public Type UnderlyingType
        {
            get { return type; }
        }

        /// <summary>
        ///   Gets the name of the type.
        /// </summary>
        public string Name
        {
            get { return type.GetPrettyName(false); }
        }

        /// <summary>
        ///   Gets the fully qualified name of the type, including the namespace but not the assembly.
        /// </summary>
        public string FullName
        {
            get { return type.GetPrettyName(true); }
        }

        /// <summary>
        ///   Gets the namespace of the type.
        /// </summary>
        public string Namespace
        {
            get { return type.Namespace; }
        }
        
        /// <inheritdoc/>
        public sealed override ProjectionFactory Factory
        {
            get { return factory; }
        }

        /// <summary>
        ///   Gets a value indicating whether virtual instances of the type can exist.
        /// </summary>
        public virtual bool IsVirtualizable
        {
            get { return false; }
        }

        /// <summary>
        ///   For a collection type, gets the type of keys used to access items in the collection;
        ///   otherwise, null.
        /// </summary>
        /// <remarks>
        ///   The type is a collection type if <see cref="TypeKindExtensions.IsCollection"/>
        ///   returns true for the value of <see cref="Kind"/>.
        /// </remarks>
        public virtual ProjectionType CollectionKeyType
        {
            get { return null; }
        }

        /// <summary>
        ///   For a collection type, gets the type of items in the collection;
        ///   otherwise, null.
        /// </summary>
        /// <remarks>
        ///   The type is a collection type if <see cref="TypeKindExtensions.IsCollection"/>
        ///   returns true for the value of <see cref="Kind"/>.
        /// </remarks>
        public virtual ProjectionType CollectionItemType
        {
            get { return null; }
        }

        /// <summary>
        ///   For a structure type, gets the base types declared by the type;
        ///   otherwise, empty.
        /// </summary>
        public virtual ProjectionTypeCollection BaseTypes
        {
            get { return ProjectionTypeCollection.Empty; }
        }

        /// <summary>
        ///   For a structure type, gets the properties declared and inherited by the type;
        ///   otherwise, empty.
        /// </summary>
        public virtual ProjectionPropertyCollection Properties
        {
            get { return ProjectionPropertyCollection.Empty; }
        }

        /// <summary>
        ///   Converts an <see cref="ProjectionType"/> to its underlying <c>System.Type</c>.
        /// </summary>
        /// <param name="projectionType">The <see cref="ProjectionType"/> to convert.</param>
        /// <returns>The underlying <c>System.Type</c> of the <see cref="ProjectionType"/>.</returns>
        public static implicit operator Type(ProjectionType projectionType)
        {
            return projectionType != null
                ? projectionType.type
                : null;
        }

        /// <summary>
        ///   Returns a string representing the name of the type.
        /// </summary>
        /// <returns>A string representing the name of the type.</returns>
        public override string ToString()
        {
            return type.GetPrettyName(true);
        }

        protected ITraitResolution TraitResolution
        {
            get { return resolution ?? (resolution = ResolveTraits()); }
        }

        private ITraitResolution ResolveTraits()
        {
            return factory.TraitResolver.Resolve(this, type);
        }
    }
}
