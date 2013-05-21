namespace Projector.Specs
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class TypeScopeTests
    {
        [Test]
        public void Property_NullName()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new TypeScope().Property(null as string)
            )
            .ForParameter("name");
        }

        [Test]
        public void Property_NullExpression()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new TypeScope<IAny>().Property(null as Expression<Func<IAny, object>>)
            )
            .ForParameter("expression");
        }
    }
}
