namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Projector.Specs;

    public sealed class StandardTraitResolverConfiguration
    {
        // TODO: Change to using some UniqueList<T> : IList<T>, ISet<T>
        // TODO: Only allocate when needed
        private UniqueCollection<Assembly>  assemblies;
        private UniqueCollection<TraitSpec> specs;

        public ICollection<Assembly> IncludedAssemblies
        {
            get { return assemblies ?? Concurrent.Ensure(ref assemblies, new UniqueCollection<Assembly>()); }
        }

        public ICollection<TraitSpec> IncludedSpecs
        {
            get { return specs ?? Concurrent.Ensure(ref specs, new UniqueCollection<TraitSpec>()); }
        }   

        internal Assembly[] GetAssembliesInternal()
        {
            var assemblies = this.assemblies;
            return assemblies == null || assemblies.Count == 0
                ? null
                : assemblies.ToArray();
        }

        internal TraitSpec[] GetSpecsInternal()
        {
            var specs = this.specs;
            return specs == null || specs.Count == 0
                ? null
                : specs.ToArray();
        }

        public StandardTraitResolverConfiguration IncludeAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw Error.ArgumentNull("assembly");

            return Add(assembly);
        }

        public StandardTraitResolverConfiguration IncludeAssemblyOf<T>()
        {
            return Add(Assembly.GetAssembly(typeof(T)));
        }

        public StandardTraitResolverConfiguration IncludeAssemblyOf(Type type)
        {
            return Add(Assembly.GetAssembly(type));
        }

        public StandardTraitResolverConfiguration IncludeSpec<TSpec>()
            where TSpec : TraitSpec, new()
        {
            return Add(TraitSpec.CreateInstance(typeof(TSpec)));
        }

        public StandardTraitResolverConfiguration IncludeSpec(TraitSpec spec)
        {
            if (spec == null)
                throw Error.ArgumentNull("spec");

            return Add(spec);
        }

        private StandardTraitResolverConfiguration Add(Assembly assembly)
        {
            var assemblies = this.assemblies;
            if (assemblies == null)
                assemblies = this.assemblies = new UniqueCollection<Assembly>();

            assemblies.Add(assembly);
            return this;
        }

        private StandardTraitResolverConfiguration Add(TraitSpec spec)
        {
            var specs = this.specs;
            if (specs == null)
                specs = this.specs = new UniqueCollection<TraitSpec>();

            specs.Add(spec);
            return this;
        }
    }
}
