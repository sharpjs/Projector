namespace Projector.ObjectModel
{
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
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

        protected static ProjectionProperty PropertyOf<T>(string name)
        {
            return PropertiesOf<T>()[name];
        }

        protected static Constraint IsSequence(params object[] items)
        {
            return Is.EqualTo(items);
        }

        protected static ProjectionType _anyType;
        protected static ProjectionType AnyType
        {
            get { return _anyType ?? (_anyType = TypeOf<IAny>()); }
        }

        protected static ProjectionType _anyStructureType;
        protected static ProjectionType AnyStructureType
        {
            get { return _anyStructureType ?? (_anyStructureType = TypeOf<IAny>()); }
        }
    }
}
