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
        public void Property_Again()
        {
            var scope = new TypeScope();

            var a = scope.Property("A");
            var b = scope.Property("A");

            Assert.That(a, Is.SameAs(b));
        }

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

        [Test]
        public void Spec_Untyped_NullSpec()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new TypeScope().Spec(null)
            )
            .ForParameter("spec");
        }

        [Test]
        public void Spec_Typed_NullSpec()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new TypeScope<IAny>().Spec(null)
            )
            .ForParameter("spec");
        }
    }
}
