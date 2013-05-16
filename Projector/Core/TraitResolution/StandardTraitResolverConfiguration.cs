namespace Projector
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Projector.Specs;

    public sealed class StandardTraitResolverConfiguration
    {
        // TODO: Change to using some UniqueList<T> : IList<T>, ISet<T>
        // TODO: Only allocate when needed
        private readonly HashSet<Assembly>   assemblySet;
        private readonly List   <Assembly>   assemblyList;
        private readonly HashSet<TraitSpec>  specSet;
        private readonly List   <TraitSpec>  specList;

        internal StandardTraitResolverConfiguration()
        {
            assemblySet  = new HashSet<Assembly>();
            specSet      = new HashSet<TraitSpec>();
            assemblyList = new List<Assembly>();
            specList     = new List<TraitSpec>();
        }

        internal IList<Assembly> IncludedAssemblies
        {
            get { return assemblyList; }
            // TODO: Implement setter
        }

        internal IList<TraitSpec> IncludedSpecs
        {
            get { return specList; }
            // TODO: Implement setter
        }

        public StandardTraitResolverConfiguration IncludeAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw Error.ArgumentNull("assembly");

            Add(assembly);
            return this;
        }

        public StandardTraitResolverConfiguration IncludeAssemblyOf<T>()
        {
            Add(Assembly.GetAssembly(typeof(T)));
            return this;
        }

        public StandardTraitResolverConfiguration IncludeAssemblyOf(Type type)
        {
            Add(Assembly.GetAssembly(type));
            return this;
        }

        public StandardTraitResolverConfiguration IncludeSpec<TSpec>()
            where TSpec : TraitSpec, new()
        {
            Add(TraitSpec.CreateInstance(typeof(TSpec)));
            return this;
        }

        public StandardTraitResolverConfiguration IncludeSpec(TraitSpec spec)
        {
            if (spec == null)
                throw Error.ArgumentNull("spec");

            Add(spec);
            return this;
        }

        private void Add(Assembly assembly)
        {
            if (assemblySet .Add(assembly))
                assemblyList.Add(assembly);
        }

        private void Add(TraitSpec spec)
        {
            if (specSet .Add(spec))
                specList.Add(spec);
        }
    }
}
