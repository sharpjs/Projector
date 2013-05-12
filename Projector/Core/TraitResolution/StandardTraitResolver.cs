namespace Projector
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Projector.ObjectModel;
    using Projector.Specs;

    public class StandardTraitResolver : ITraitResolver
    {
        private readonly Assembly [] assemblies;
        private readonly TraitSpec[] specs;

        public StandardTraitResolver() : this(null) { }

        public StandardTraitResolver(Action<StandardTraitResolverConfiguration> configure)
            : this()
        {
            if (configure != null)
            {
                var configuration = new StandardTraitResolverConfiguration();
                configure(configuration);

                assemblies = configuration.GetAssemblies();
                specs      = configuration.GetSpecs();
            }
            else
            {
                assemblies = new Assembly [0];
                specs      = new TraitSpec[0];
            }
        }

        public ITraitResolution Resolve(ProjectionType projectionType, Type underlyingType)
        {
            if (projectionType == null)
                throw Error.ArgumentNull("projectionType");
            if (underlyingType == null)
                throw Error.ArgumentNull("underlyingType");

            var resolution = new StandardTraitResolution(projectionType, underlyingType);

            AddIncludedSpecs(resolution);
            AddDetectedSpecs(resolution, GetSharedSpecName (underlyingType));
            AddDetectedSpecs(resolution, GetPerTypeSpecName(underlyingType));

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

                var spec = CreateSpec(type);
                if (spec == null)
                    continue;

                resolution.Add(spec);
            }
        }

        private string GetSharedSpecName(Type type)
        {
            return string.Concat
            (
                type.Namespace,
                Separator + SharedSpecName // compile-time constant
            );
        }

        private string GetPerTypeSpecName(Type type)
        {
            return string.Concat
            (
                type.Namespace,
                Separator,
                type.Name.RemoveInterfacePrefix(),
                PerTypeSpecSuffix
            );
        }

        private TraitSpec CreateSpec(Type type)
        {
            try
            {
                return (TraitSpec) Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        private const string
            Separator         = ".",
            SharedSpecName    = "SharedTraits",
            PerTypeSpecSuffix = "Traits";
    }
}
