namespace Projector.ObjectModel
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    // TODO: Move this to root Projector namespace
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

        protected static ProjectionProperty PropertyOf<T>(Expression<Func<T, object>> expression)
        {
            return PropertyOf<T>(expression.ToProperty().Name);
        }

        protected static ProjectionProperty PropertyOf<T>(string name)
        {
            return PropertiesOf<T>()[name];
        }

        protected static Constraint IsSequence(params object[] items)
        {
            return Is.EqualTo(items);
        }
    }
}
