namespace Projector
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Projector.ObjectModel;

    public class ProjectionFactory
    {
        private static ProjectionFactory defaultInstance;

        private readonly Dictionary<Type, ProjectionType> types;
        private readonly CellList<ProjectionType>         incompleteTypes;
        private readonly ReaderWriterLockSlim             typesLock;
        private readonly ProjectionAssemblyFactory        assemblyFactory;
        //private readonly ProjectionProvider[]             providers;
        //private readonly ProjectionProviderCollection     providersPublic;
        private readonly ITraitResolver                   resolver;

        public ProjectionFactory()
            : this(new ProjectionConfiguration()) { }

        public ProjectionFactory(Action<ProjectionConfiguration> configure)
            : this(Configure(configure)) { }

        public ProjectionFactory(IProjectionConfiguration configuration)
        {
            if (configuration == null)
                throw Error.ArgumentNull("configuration");

            //providers       = configuration.GetProviders();
            resolver        = ProjectionConfiguration.GetTraitResolver(configuration);
            assemblyFactory = CreateAssemblyFactory                   (configuration);
            types           = new Dictionary<Type, ProjectionType>();
            typesLock       = new ReaderWriterLockSlim();
            incompleteTypes = new CellList<ProjectionType>();
            //providersPublic = new ProjectionProviderCollection(providers);
        }

        private static ProjectionConfiguration Configure(Action<ProjectionConfiguration> configure)
        {
            if (configure == null)
                throw Error.ArgumentNull("configure");

            var configuration = new ProjectionConfiguration();
            configure(configuration);
            return configuration;
        }

        private static ProjectionAssemblyFactory CreateAssemblyFactory(IProjectionConfiguration configuration)
        {
            var options   = ProjectionConfiguration.GetOptions            (configuration);
            var threshold = ProjectionConfiguration.GetMaxTypesPerAssembly(configuration);

            return threshold <= 0 ? ProjectionAssemblyFactory.Single (options)
                 : threshold == 1 ? ProjectionAssemblyFactory.PerType(options)
                 : ProjectionAssemblyFactory.Batched(threshold, options);
        }

        /// <summary>
        ///   Gets the default projection factory.
        /// </summary>
        /// <remarks>
        ///   The default projection factory takes default values for all configurable parameters.
        /// </remarks>
        public static ProjectionFactory Default
        {
            get
            {
                return defaultInstance
                    ?? Concurrent.Ensure(ref defaultInstance, new ProjectionFactory());
            }
        }

        ///// <summary>
        /////   Gets the projection providers that are configured for the factory.
        ///// </summary>
        //public ProjectionProviderCollection Providers
        //{
        //    get { return providersPublic; }
        //}

        /// <summary>
        ///   Gets the trait resolver that is configured for the factory.
        /// </summary>
        public ITraitResolver TraitResolver
        {
            get { return resolver; }
        }

        public T Create<T>()
        {
            return (T) CreateCore(typeof(T), null, 0, null, null);
        }

        //public T Create<T>(object store)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return (T) CreateCore(typeof(T), null, token, store, null);
        //}

        //public T Create<T>(object store, params object[] args)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return (T) CreateCore(typeof(T), null, token, store, args);
        //}

        //public T Create<T>(object context)
        //{
        //    return (T) CreateCore(typeof(T), Require(context), 0, null, null);
        //}

        //public T Create<T>(object context, object store)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return (T) CreateCore(typeof(T), Require(context), token, store, null);
        //}

        //public T Create<T>(object context, object store, params object[] args)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return (T) CreateCore(typeof(T), Require(context), token, store, args);
        //}

        //public object Create(Type type)
        //{
        //    return CreateCore(type, null, 0, null, null);
        //}

        //public object Create(Type type, object store)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return CreateCore(type, null, token, store, null);
        //}

        //public object Create(Type type, object store, params object[] args)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return CreateCore(type, null, token, store, args);
        //}

        //public object Create(Type type, object context)
        //{
        //    return CreateCore(type, Require(context), 0, null, null);
        //}

        //public object Create(Type type, object context, object store)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return CreateCore(type, Require(context), token, store, null);
        //}

        //public object Create(Type type, object context, object store, params object[] args)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return CreateCore(type, Require(context), token, store, args);
        //}

        private object CreateCore(Type type, object context, int token, object store, object[] args)
        {
            return new ProjectionInstance(this, token, store, context)
                .Initialize(type, args);
        }

        //private int FindStorageProvider(ref object store)
        //{
        //    if (store == null)
        //        return 0; // Default to first provider

        //    for (var i = 0; i < providers.Length; i++)
        //        if (providers[i].TryConsume(ref store))
        //            return i;

        //    throw Error.NoStorageProvider();
        //}

        public ProjectionType GetProjectionType(Type type)
        {
            ProjectionType projectionType;

            if (type == null)
                throw Error.ArgumentNull("type");

            typesLock.EnterReadLock();
            try
            {
                if (types.TryGetValue(type, out projectionType))
                    return projectionType;
            }
            finally
            {
                typesLock.ExitReadLock();
            }

            typesLock.EnterUpgradeableReadLock();
            try
            {
                if (types.TryGetValue(type, out projectionType))
                    return projectionType;

                typesLock.EnterWriteLock();
                try
                {
                    return InitializeProjectionTypes(type);
                }
                catch
                {
                    RemoveIncompleteTypes();
                    throw;
                }
                finally
                {
                    typesLock.ExitWriteLock();
                }
            }
            finally
            {
                typesLock.ExitUpgradeableReadLock();
            }
        }

        private ProjectionType InitializeProjectionTypes(Type type)
        {
            var projectionType = CreateProjectionType(type);

            Cell<ProjectionType> cell;

            cell = incompleteTypes.Head;
            do cell.Item.ComputeTraits();
            while (null != (cell = cell.Next));

            cell = incompleteTypes.Head;
            do cell.Item.FreezeTraits();
            while (null != (cell = cell.Next));

            cell = incompleteTypes.Head;
            do cell.Item.InvokeInitializers();
            while (null != (cell = cell.Next));

            cell = incompleteTypes.Head;
            do cell.Item.InvokePostInitializers();
            while (null != (cell = cell.Next));

            incompleteTypes.Clear();
            return projectionType;
        }

        internal ProjectionType GetProjectionTypeUnsafe(Type type)
        {
            ProjectionType projectionType;
            return types.TryGetValue(type, out projectionType)
                ? projectionType
                : CreateProjectionType(type);
        }

        private ProjectionType CreateProjectionType(Type type)
        {
            switch (type.Classify())
            {
                default:
                case TypeKind.Opaque:     return new ProjectionOpaqueType    (type, this);
                case TypeKind.Structure:  return new ProjectionStructureType (type, this);
                case TypeKind.Array:      return new ProjectionArrayType     (type, this);
                case TypeKind.List:       return new ProjectionListType      (type, this);
                case TypeKind.Set:        return new ProjectionSetType       (type, this);
                case TypeKind.Dictionary: return new ProjectionDictionaryType(type, this);
            }
        }

        internal void RegisterProjectionType(ProjectionType projectionType)
        {
            incompleteTypes.Enqueue(projectionType);
            types[projectionType.UnderlyingType] = projectionType;
        }

        private void RemoveIncompleteTypes()
        {
            ProjectionType projectionType;
            while (incompleteTypes.TryTake(out projectionType))
                types.Remove(projectionType);
        }

        internal ProjectionConstructor ImplementProjectionType(ProjectionStructureType projectionType)
        {
            return assemblyFactory
                .GetAssembly(projectionType.UnderlyingType)
                .ImplementProjectionClass(projectionType);
        }

        internal void SaveGeneratedAssemblies()
        {
            assemblyFactory.SaveAll();
        }
    }
}
