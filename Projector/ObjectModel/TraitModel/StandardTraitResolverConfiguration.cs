namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Projector.Specs;
    using Projector.Utility;

    public sealed class StandardTraitResolverConfiguration : IStandardTraitResolverConfiguration, IFluent
    {
        private List<Assembly>  assemblies;
        private List<TraitSpec> specs;

        ICollection<Assembly> IStandardTraitResolverConfiguration.IncludedAssemblies
        {
            get { return assemblies; }
        }

        ICollection<TraitSpec> IStandardTraitResolverConfiguration.IncludedSpecs
        {
            get { return specs; }
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

        public StandardTraitResolverConfiguration IncludeSpec(TraitSpec spec)
        {
            if (spec == null)
                throw Error.ArgumentNull("spec");

            return Add(spec);
        }

        public StandardTraitResolverConfiguration IncludeSpec<TSpec>()
            where TSpec : TraitSpec, new()
        {
            return Add(TraitSpec.CreateInstance(typeof(TSpec)));
        }

        private StandardTraitResolverConfiguration Add(Assembly assembly)
        {
            var assemblies = this.assemblies;
            if (assemblies == null)
                assemblies = this.assemblies = new List<Assembly>();

            assemblies.Add(assembly);
            return this;
        }

        private StandardTraitResolverConfiguration Add(TraitSpec spec)
        {
            var specs = this.specs;
            if (specs == null)
                specs = this.specs = new List<TraitSpec>();

            specs.Add(spec);
            return this;
        }

        internal static Assembly[] GetIncludedAssemblies(IStandardTraitResolverConfiguration configuration)
        {
            return configuration.IncludedAssemblies.ToUniqueArrayOrNull(new ObjectEqualityComparer<Assembly>());
            // PERF: Don't use comparer .Instance; no need to keep it after this.
        }

        internal static TraitSpec[] GetIncludedSpecs(IStandardTraitResolverConfiguration configuration)
        {
            return configuration.IncludedSpecs.ToUniqueArrayOrNull(new ReferenceEqualityComparer<TraitSpec>());
            // PERF: Don't use comparer .Instance; no need to keep it after this.
        }
    }
}
