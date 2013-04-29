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

        //protected static ProjectionTypeCollection<ProjectionStructureType> BaseStructureTypesOf<T>()
        //{
        //    return ProjectionTypeOf<T>().BaseStructureTypes;
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
