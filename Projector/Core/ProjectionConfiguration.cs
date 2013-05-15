namespace Projector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Projector.ObjectModel;

    public sealed class ProjectionConfiguration
    {
        //private ProjectionProvider[] providers;
        private ITraitResolver       resolver;
        //private ProjectionOptions    options;

        public ProjectionConfiguration() { }

        //public ProjectionConfiguration SaveAssemblies()
        //{
        //    return SetAssemblyMode(ProjectionOptions.SaveAssembly);
        //}

        //public ProjectionConfiguration CollectAssemblies()
        //{
        //    return SetAssemblyMode(ProjectionOptions.CollectAssembly);
        //}

        //private ProjectionConfiguration SetAssemblyMode(ProjectionOptions value)
        //{
        //    if (options.Any(ProjectionOptionsInternal.AssemblyModes))
        //        throw Error.AssemblyModeAlreadyConfigured();

        //    options |= value;
        //    return this;
        //}

        //internal ProjectionOptions GetOptions()
        //{
        //    return options;
        //}

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

        public ITraitResolver TraitResolver
        {
            get { return resolver ?? (resolver = new StandardTraitResolver()); }
            set
            {
                if (value == null)
                    throw Error.ArgumentNull("value");

                resolver = value;
            }
        }

        public ProjectionConfiguration WithTraitResolver(ITraitResolver resolver)
        {
            TraitResolver = resolver;
            return this;
        }
    }
}
