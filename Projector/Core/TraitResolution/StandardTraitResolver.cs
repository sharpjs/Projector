namespace Projector
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Projector.ObjectModel;
    using Projector.Specs;

    public class StandardTraitResolver : ITraitResolver
    {
        private static StandardTraitResolver defaultInstance;

        private readonly Assembly [] assemblies;
        private readonly TraitSpec[] specs;

        public StandardTraitResolver()
            : this(new StandardTraitResolverConfiguration()) { }

        public StandardTraitResolver(Action<StandardTraitResolverConfiguration> configure)
            : this(Configure(configure)) { }

        public StandardTraitResolver(StandardTraitResolverConfiguration configuration)
        {
            if (configuration == null)
                throw Error.ArgumentNull("configuration");

            assemblies = configuration.IncludedAssemblies.ToArray();
            specs      = configuration.IncludedSpecs     .ToArray();
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
            get
            {
                return defaultInstance
                    ?? Concurrent.Ensure(ref defaultInstance, new StandardTraitResolver());
            }
        }

        public ITraitResolution Resolve(ProjectionType projectionType, Type underlyingType)
        {
            if (projectionType == null)
                throw Error.ArgumentNull("projectionType");
            if (underlyingType == null)
                throw Error.ArgumentNull("underlyingType");

            var resolution = new StandardTraitResolution(projectionType, underlyingType);

            if (specs != null)
            {
                AddIncludedSpecs(resolution);
            }

            if (assemblies != null)
            {
                AddDetectedSpecs(resolution, GetSharedSpecName(underlyingType));
                AddDetectedSpecs(resolution, GetPerTypeSpecName(underlyingType));
            }

            return resolution;
        }

        private void AddIncludedSpecs(StandardTraitResolution resolution)
        {
            foreach (var spec in specs)
                resolution.Add(spec);
        }

        private void AddDetectedSpecs(StandardTraitResolution resolution, string name)
        {
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(name);
                if (type == null || !type.IsSubclassOf(typeof(TraitSpec)))
                    continue;

                var spec = TraitSpec.CreateInstance(type);
                if (spec == null)
                    continue;

                resolution.Add(spec);
            }
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
