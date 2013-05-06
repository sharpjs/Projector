namespace Projector
{
    using System;
    using System.Reflection;
    using Projector.ObjectModel;
    using Projector.Specs;

    public class StandardTraitResolver : ITraitResolver
    {
        private readonly Assembly[]  assemblies;
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
                assemblies = new Assembly[0];
                specs      = new TraitSpec[0];
            }
        }

        public void Resolve(ProjectionType target, TraitApplicator applicator)
        {
            var type = target.UnderlyingType;
            ApplyIncludedSpecs(target, applicator);
            ApplyResolvedSpecs(target, applicator, GetSharedSpecName (type));
            ApplyResolvedSpecs(target, applicator, GetPerTypeSpecName(type));
        }

        private void ApplyIncludedSpecs(ProjectionType target, TraitApplicator applicator)
        {
            foreach (var spec in specs)
                spec.Apply(target, applicator);
        }

        private void ApplyResolvedSpecs(ProjectionType target, TraitApplicator applicator, string name)
        {
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(name);
                if (type == null || !type.IsSubclassOf(typeof(TraitSpec)))
                    continue;

                var spec = CreateSpec(type);
                if (spec == null)
                    continue;

                spec.Apply(target, applicator);
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
