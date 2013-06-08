namespace Projector.ObjectModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Projector.Specs;

    public class StandardTraitResolver : ITraitResolver
    {
        private static StandardTraitResolver defaultInstance;

        private readonly Assembly []          assemblies;
        private readonly TraitSpec[]          specs;
        private ReadOnlyCollection<Assembly>  assembliesPublic;
        private ReadOnlyCollection<TraitSpec> specsPublic;

        public StandardTraitResolver()
            : this(new StandardTraitResolverConfiguration()) { }

        public StandardTraitResolver(Action<StandardTraitResolverConfiguration> configure)
            : this(Configure(configure)) { }

        public StandardTraitResolver(IStandardTraitResolverConfiguration configuration)
        {
            if (configuration == null)
                throw Error.ArgumentNull("configuration");

            assemblies = StandardTraitResolverConfiguration.GetIncludedAssemblies(configuration);
            specs      = StandardTraitResolverConfiguration.GetIncludedSpecs     (configuration);
        }

        private static StandardTraitResolverConfiguration
        Configure(Action<StandardTraitResolverConfiguration> configure)
        {
            if (configure == null)
                throw Error.ArgumentNull("configure");

            var configuration = new StandardTraitResolverConfiguration();
            configure(configuration);
            return configuration;
        }

        /// <summary>
        ///   Gets the default trait resolver.
        /// </summary>
        /// <remarks>
        ///   The default trait resolver takes default values for all configurable parameters.
        /// </remarks>
        public static StandardTraitResolver Default
        {
            get { return defaultInstance ?? Concurrent.Ensure(ref defaultInstance, new StandardTraitResolver()); }
        }

        public ReadOnlyCollection<Assembly> IncludedAssemblies
        {
            get { return assembliesPublic ?? Concurrent.Ensure(ref assembliesPublic, Array.AsReadOnly(assemblies ?? new Assembly[0])); }
        }

        public ReadOnlyCollection<TraitSpec> IncludedSpecs
        {
            get { return specsPublic ?? Concurrent.Ensure(ref specsPublic, Array.AsReadOnly(specs ?? new TraitSpec[0])); }
        }

        public ITraitResolution Resolve(ProjectionType projectionType, Type underlyingType)
        {
            if (projectionType == null)
                throw Error.ArgumentNull("projectionType");
            if (underlyingType == null)
                throw Error.ArgumentNull("underlyingType");

            var resolution = new StandardTraitResolution(projectionType, underlyingType);
            var assembly   = underlyingType.Assembly;

            AddIncludedSpecs(resolution);
            AddDetectedSpecs(resolution, GetSharedSpecName (underlyingType), assembly);
            AddDetectedSpecs(resolution, GetPerTypeSpecName(underlyingType), assembly);

            return resolution;
        }

        private void AddIncludedSpecs(StandardTraitResolution resolution)
        {
            var specs = this.specs;
            if (specs != null)
                foreach (var spec in specs)
                    resolution.Add(spec);
        }

        private void AddDetectedSpecs(StandardTraitResolution resolution, string name, Assembly containingAssembly)
        {
            var visitedContainingAssembly = false;

            var assemblies = this.assemblies;
            if (assemblies != null)
            {
                foreach (var assembly in assemblies)
                {
                    if (assembly == containingAssembly)
                        visitedContainingAssembly = true;
                    AddDetectedSpec(resolution, name, assembly);
                }
            }

            if (!visitedContainingAssembly)
                AddDetectedSpec(resolution, name, containingAssembly);
        }

        private static void AddDetectedSpec(StandardTraitResolution resolution, string name, Assembly assembly)
        {
            var type = assembly.GetType(name);
            if (type == null || !type.IsSubclassOf(typeof(TraitSpec)))
                return;

            var spec = TraitSpec.CreateInstance(type);
            if (spec == null)
                return;

            resolution.Add(spec);
        }

        private static string GetSharedSpecName(Type type)
        {
            return string.Concat
            (
                type.Namespace,
                Separator + SharedSpecName // compile-time constant
            );
        }

        private static string GetPerTypeSpecName(Type type)
        {
            return string.Concat
            (
                type.Namespace,
                Separator,
                type.Name.RemoveInterfacePrefix(),
                PerTypeSpecSuffix
            );
        }

        private const string
            Separator         = ".",
            SharedSpecName    = "SharedTraits",
            PerTypeSpecSuffix = "Traits";
    }
}
