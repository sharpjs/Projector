namespace Projector
{
    using System;
    using Projector.ObjectModel;
    using Projector.Utility;

    [Serializable]
    public sealed class ProjectionConfiguration : IProjectionConfiguration, IFluent
    {
        #region Options

        private ProjectionOptions options;

        ProjectionOptions IProjectionConfiguration.Options
        {
            get { return options; }
        }

        public ProjectionConfiguration EnableSaveAssemblies()
        {
            options = options.Set(ProjectionOptions.SaveAssemblies, true);
            return this;
        }

        public ProjectionConfiguration EnableSaveAssemblies(bool enabled)
        {
            options = options.Set(ProjectionOptions.SaveAssemblies, enabled);
            return this;
        }

        public ProjectionConfiguration EnableCollectAssemblies()
        {
            options = options.Set(ProjectionOptions.CollectAssemblies, true);
            return this;
        }

        public ProjectionConfiguration EnableCollectAssemblies(bool enabled)
        {
            options = options.Set(ProjectionOptions.CollectAssemblies, enabled);
            return this;
        }

        internal static ProjectionOptions GetOptions(IProjectionConfiguration configuration)
        {
            var options = configuration.Options;

            switch (options & ProjectionOptionsInternal.AssemblyModes)
            {
                case ProjectionOptions.None:
                case ProjectionOptions.SaveAssemblies:
                case ProjectionOptions.CollectAssemblies:
                    return options;
                default:
                    throw Error.SaveAndCollectAssembliesNotSupported();
            }
        }

        #endregion

        #region DynamicAssemblyMemberLimit

        internal const int
            SingleAssembly  = 0,
            AssemblyPerType = 1;

        private int? maxTypesPerAssembly;

        int? IProjectionConfiguration.MaxTypesPerAssembly
        {
            get { return maxTypesPerAssembly; }
        }

        public ProjectionConfiguration GenerateSingleAssembly()
        {
            maxTypesPerAssembly = SingleAssembly;
            return this;
        }

        public ProjectionConfiguration GenerateAssemblyPerType()
        {
            maxTypesPerAssembly = AssemblyPerType;
            return this;
        }

        public ProjectionConfiguration GenerateAssemblyPerNTypes(int threshold)
        {
            maxTypesPerAssembly = threshold;
            return this;
        }

        internal static int GetMaxTypesPerAssembly(IProjectionConfiguration configuration)
        {
            return configuration.MaxTypesPerAssembly ??
            (
                // In some versions of .NET, performance of TypeBuilder.CreateType degrades at O(n^2) as more
                // types are added to the dynamic assembly.  Workaround is to create one assembly per N types,
                // where N is a low-enough number.
                //
                // See: http://support.microsoft.com/kb/970924
                //
                Environment.Version.Major < 4 ? 4 : 64
            );
        }

        internal static ProjectionAssemblyFactory GetAssemblyFactory(IProjectionConfiguration configuration)
        {
            var options = GetOptions(configuration);
            var threshold = configuration.MaxTypesPerAssembly;

            return threshold <= 0 ? ProjectionAssemblyFactory.Single (options)
                 : threshold == 1 ? ProjectionAssemblyFactory.PerType(options)
                 : ProjectionAssemblyFactory.Batched(threshold.Value, options);
        }

        #endregion

        #region Providers

        //private ProjectionProvider[] providers;

        //public ProjectionConfiguration Providers(IEnumerable<ProjectionProvider> providers)
        //{
        //    if (providers == null)
        //        throw Error.ArgumentNull("providers");
        //    if (this.providers != null)
        //        throw Error.ProvidersAlreadyConfigured();

        //    var array = providers
        //        .DistinctOrdered(ReferenceEqualityComparer<ProjectionProvider>.Instance)
        //        .ToArray();

        //    if (array.Length == 0)
        //        throw Error.NoStorageProviders();

        //    this.providers = array;
        //    return this;
        //}

        //public ProjectionConfiguration Providers(params ProjectionProvider[] providers)
        //{
        //    return Providers(providers as IEnumerable<ProjectionProvider>);
        //}

        //internal ProjectionProvider[] GetProviders()
        //{
        //    return providers ?? new ProjectionProvider[]
        //    {
        //        DictionaryStorageProvider.Instance
        //    };
        //}

        #endregion

        #region TraitResolver

        private ITraitResolver resolver;

        ITraitResolver IProjectionConfiguration.TraitResolver
        {
            get { return resolver; }
        }

        public ProjectionConfiguration UseTraitResolver(ITraitResolver resolver)
        {
            this.resolver = resolver;
            return this;
        }

        public ProjectionConfiguration UseStandardTraitResolver()
        {
            resolver = new StandardTraitResolver();
            return this;
        }

        public ProjectionConfiguration UseStandardTraitResolver(Action<StandardTraitResolverConfiguration> configure)
        {
            resolver = new StandardTraitResolver(configure);
            return this;
        }

        public ProjectionConfiguration UseStandardTraitResolver(IStandardTraitResolverConfiguration configuration)
        {
            resolver = new StandardTraitResolver(configuration);
            return this;
        }

        internal static ITraitResolver GetTraitResolver(IProjectionConfiguration configuration)
        {
            return configuration.TraitResolver ?? new StandardTraitResolver();
        }

        #endregion
    }
}
