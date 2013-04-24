namespace Projector.ObjectModel
{
    using Projector.ObjectModel;

    public abstract class ProjectionTestsBase
    {
        protected static ProjectionFactory Factory
        {
            get { return ProjectionFactory.Default; }
        }

        protected static ProjectionType TypeOf<T>()
        {
            return Factory.GetProjectionType(typeof(T));
        }

        //protected static ProjectionStructureType ProjectionTypeOf<T>()
        //{
        //    return TypeOf<T>().AsProjectionType;
        //}

        //protected static ProjectionCollectionType CollectionTypeOf<T>()
        //{
        //    return TypeOf<T>().AsCollectionType;
        //}

        //protected static ProjectionTypeCollection<ProjectionStructureType> BaseTypesOf<T>()
        //{
        //    return ProjectionTypeOf<T>().BaseTypes;
        //}

        //protected static ProjectionPropertyCollection PropertiesOf<T>()
        //{
        //    return ProjectionTypeOf<T>().Properties;
        //}

        //protected static ProjectionProperty PropertyOf<T>(string name)
        //{
        //    return PropertiesOf<T>()[name];
        //}
    }
}
