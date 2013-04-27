namespace Projector
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    //using Projector.Configuration;
    using Projector.ObjectModel;

    public class ProjectionFactory
    {
        private static ProjectionFactory defaultFactory;

        private readonly Dictionary<Type, ProjectionType> types;
        private readonly CellList<ProjectionType>         incompleteTypes;
        private readonly ReaderWriterLockSlim             typesLock;
        //private readonly ProjectionAssemblyFactory        assemblyFactory;
        //private readonly ProjectionProvider[]             providers;
        //private readonly ProjectionProviderCollection     providersPublic;
        //private readonly ITraitResolver                   resolver;
        //private readonly ProjectionOptions                options;

        //public ProjectionFactory() : this(null) { }

        public ProjectionFactory(/*Action<ProjectionConfiguration> configure*/)
        {
            //var configuration = new ProjectionConfiguration(this);
            //if (configure != null)
            //    configure(configuration);

            //options          = configuration.GetOptions();
            //providers        = configuration.GetProviders();
            //resolver         = configuration.GetTraitResolver();

            types            = new Dictionary<Type, ProjectionType>();
            typesLock        = new ReaderWriterLockSlim();
            incompleteTypes  = new CellList<ProjectionType>();
            //assemblyFactory  =     ProjectionAssemblyFactory.PerType(options);
            //providersPublic  = new ProjectionProviderCollection(providers);
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
                return defaultFactory != null
                    ? defaultFactory
                    : Concurrent.Ensure(ref defaultFactory, new ProjectionFactory());
            }
        }

        ///// <summary>
        /////   Gets the projection providers that are configured for the factory.
        ///// </summary>
        //public ProjectionProviderCollection Providers
        //{
        //    get { return providersPublic; }
        //}

        //public T Create<T>()
        //{
        //    return (T) CreateCore(typeof(T), ProjectionContext.Default, 0, null, null);
        //}

        //public T Create<T>(object store)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return (T) CreateCore(typeof(T), ProjectionContext.Default, token, store, null);
        //}

        //public T Create<T>(object store, params object[] args)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return (T) CreateCore(typeof(T), ProjectionContext.Default, token, store, args);
        //}

        //public T Create<T>(IProjectionContext context)
        //{
        //    return (T) CreateCore(typeof(T), Require(context), 0, null, null);
        //}

        //public T Create<T>(IProjectionContext context, object store)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return (T) CreateCore(typeof(T), Require(context), token, store, null);
        //}

        //public T Create<T>(IProjectionContext context, object store, params object[] args)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return (T) CreateCore(typeof(T), Require(context), token, store, args);
        //}

        //public object Create(Type type)
        //{
        //    return CreateCore(type, ProjectionContext.Default, 0, null, null);
        //}

        //public object Create(Type type, object store)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return CreateCore(type, ProjectionContext.Default, token, store, null);
        //}

        //public object Create(Type type, object store, params object[] args)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return CreateCore(type, ProjectionContext.Default, token, store, args);
        //}

        //public object Create(Type type, IProjectionContext context)
        //{
        //    return CreateCore(type, Require(context), 0, null, null);
        //}

        //public object Create(Type type, IProjectionContext context, object store)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return CreateCore(type, Require(context), token, store, null);
        //}

        //public object Create(Type type, IProjectionContext context, object store, params object[] args)
        //{
        //    var token = FindStorageProvider(ref store);
        //    return CreateCore(type, Require(context), token, store, args);
        //}

        //private object CreateCore(Type type, IProjectionContext context, int token, object store, object[] args)
        //{
        //    return new ProjectionInstance(this, context, token, store)
        //        .Initialize(type, args);
        //}

        //private int FindStorageProvider(ref object store)
        //{
        //    if (store == null)
        //        return 0; // Default to first provider

        //    for (var i = 0; i < providers.Length; i++)
        //        if (providers[i].TryConsume(ref store))
        //            return i;

        //    throw Error.NoStorageProvider();
        //}

        //private static IProjectionContext Require(IProjectionContext context)
        //{
        //    if (context != null)
        //        return context;

        //    throw Error.ArgumentNull("context");
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
            do cell.Item.InvokeLateInitializers();
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

        //internal ProjectionConstructor ImplementProjectionType(ProjectionStructureType projectionType)
        //{
        //    return assemblyFactory
        //        .GetAssembly(projectionType.UnderlyingType)
        //        .ImplementProjectionClass(projectionType);
        //}

        //internal void SaveGeneratedAssemblies()
        //{
        //    assemblyFactory.SaveAll();
        //}
    }
}
