﻿namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    internal sealed class ProjectionStructureType : ProjectionType
    {
        private readonly ProjectionTypeCollection     baseTypes;
        private readonly ProjectionPropertyCollection properties;

        //private ProjectionConstructor constructor;
        //private object                constructorLock;
        private object[]              inheritableTraits;

        internal ProjectionStructureType(Type type, ProjectionFactory factory)
            : base(type, TypeKind.Structure, factory)
        {
            // Ensure base types exist before self
            //   - Safe because no type can have itself as a base (i.e. no cycles)
            //   - Causes inits to be called in order from more-base to more-derived
            int basePropertyCount;
            baseTypes = CollectBaseTypes(type, out basePropertyCount);

            // Register self
            //   - Required before creating metaobjects that can refer back to this type
            factory.RegisterProjectionType(this);

            // Create members after registration
            //   - Members can be of any type and thus can form cycles
            properties = CollectProperties(type, basePropertyCount);
        }

        internal override void InitializePass1()
        {
            inheritableTraits = CollectTraitsCore();

            foreach (var property in properties)
                if (property.DeclaringType == this)
                    property.InitializePass1();
        }

        internal override void InitializePass3()
        {
            InvokeInitializersCore();

            foreach (var property in properties)
                if (property.DeclaringType == this)
                    property.InitializePass3();
        }

        internal override void InitializePass4()
        {
            InvokeLateInitializersCore();

            foreach (var property in properties)
                if (property.DeclaringType == this)
                    property.InitializePass4();
        }

        /// <summary>
        /// Gets the base types declared and inherited by the type.
        /// </summary>
        public override ProjectionTypeCollection BaseTypes
        {
            get { return baseTypes; }
        }

        /// <summary>
        /// Gets the properties declared and inherited by the type.
        /// </summary>
        public override ProjectionPropertyCollection Properties
        {
            get { return properties; }
        }

        /// <inheritdoc/>
        public override bool IsVirtualizable
        {
            get { return true; }
        }

        // Traits inherited by derived types
        internal object[] InheritableTraits
        {
            get { return inheritableTraits; }
        }

        //internal Projection CreateProjection(ProjectionInstance instance)
        //{
        //    ProjectionConstructor create;

        //    if ((create = constructor) == null)
        //    {
        //        var newLock = new object();
        //        var theLock = Interlocked.CompareExchange(ref constructorLock, newLock, null) ?? newLock;

        //        Monitor.Enter(theLock);
        //        try
        //        {
        //            if ((create = constructor) == null)
        //                create = constructor = Factory.ImplementProjectionType(this);
        //        }
        //        finally
        //        {
        //            constructorLock = null;
        //            Monitor.Exit(theLock);
        //        }
        //    }

        //    return create(instance);
        //}

        private ProjectionTypeCollection CollectBaseTypes(Type type, out int propertyCount)
        {
            var interfaces = type.GetInterfaces();
            if (interfaces.Length != 0)
            {
                var flattenedBaseTypes = GetProjectionTypes(interfaces, Factory);
                var immediateBaseTypes = GetImmediateBaseTypes(flattenedBaseTypes);
                return CollectBaseTypesCore(immediateBaseTypes, out propertyCount);
            }
            else
            {
                propertyCount = 0;
                return ProjectionTypeCollection.Empty;
            }
        }

        private static ProjectionStructureType[] GetProjectionTypes(Type[] interfaces, ProjectionFactory factory)
        {
            var baseTypes = new ProjectionStructureType[interfaces.Length];

            for (var i = 0; i < interfaces.Length; i++)
            {
                var baseType = baseTypes[i] =
                    factory.GetProjectionTypeUnsafe(interfaces[i])
                    as ProjectionStructureType;
                if (baseType == null)
                    throw Error.InvalidProjectionType(interfaces[i]);
            }

            return baseTypes;
        }

        private static HashSet<ProjectionType> GetImmediateBaseTypes(ProjectionStructureType[] baseTypes)
        {
            var result = new HashSet<ProjectionType>();

            foreach (var baseType in baseTypes)
                result.Add(baseType);

            foreach (var baseType in baseTypes)
                result.ExceptWith(baseType.BaseTypes);

            return result;
        }

        private static ProjectionTypeCollection CollectBaseTypesCore(
            HashSet<ProjectionType> baseTypes, out int propertyCount)
        {
            var collection = new ProjectionTypeCollection(baseTypes.Count);
            var count      = 0;

            foreach (ProjectionStructureType baseType in baseTypes)
            {
                count += baseType.properties.Count;
                collection.Add(baseType);
            }

            propertyCount = count;
            return collection;
        }

        private ProjectionPropertyCollection CollectProperties(Type type, int basePropertyCount)
        {
            var properties = type.GetProperties();
            var count      = properties.Length + basePropertyCount;
            if (count != 0)
            {
                var collection = new ProjectionPropertyCollection(count);

                CollectInheritedProperties(baseTypes,  collection);
                CollectDeclaredProperties (properties, collection); // MUST come after inherited

                return collection;
            }
            else
            {
                return ProjectionPropertyCollection.Empty;
            }
        }

        private void CollectDeclaredProperties(PropertyInfo[] properties, ProjectionPropertyCollection collection)
        {
            var factory = Factory;
            foreach (var property in properties)
                collection.Add(new ProjectionProperty(property, this, collection, factory), true);
        }

        private static void CollectInheritedProperties(ProjectionTypeCollection baseTypes, ProjectionPropertyCollection collection)
        {
            foreach (var baseType in baseTypes)
            foreach (var property in baseType.Properties)
                collection.Add(property, false);
        }

        private object[] CollectTraitsCore()
        {
            var aggregator = new ProjectionTypeTraitAggregator(this);

            aggregator.CollectDeclaredTraits();
            aggregator.CollectInheritedTraits();
            aggregator.ApplyDeferredTraits();

            return aggregator.InheritableTraits;
        }

        private void InvokeInitializersCore()
        {
            //var traits = this.GetTraits(profile);

            //new TypeInitializerInvocation
            //    (this, traits, Traits.FirstBehavior)
            //    .Proceed();
        }

        private void InvokeLateInitializersCore()
        {
            //new TypeLateInitializerInvocation
            //    (this, traits, Traits.FirstBehavior)
            //    .Proceed();
        }
    }
}
