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

        protected static ProjectionTypeCollection BaseTypesOf<T>()
        {
            return TypeOf<T>().BaseTypes;
        }

        protected static ProjectionPropertyCollection PropertiesOf<T>()
        {
            return TypeOf<T>().Properties;
        }

        //protected static ProjectionStructureType ProjectionTypeOf<T>()
        //{
        //    return (ProjectionStructureType) TypeOf<T>();
        //}

        protected static ProjectionProperty PropertyOf<T>(string name)
        {
            return PropertiesOf<T>()[name];
        }
    }
}
