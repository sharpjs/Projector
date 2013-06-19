namespace Projector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture, Explicit]
    public class ExperimentTests
    {
        // This space for experiments

        [Test]
        public void AccessAProperty()
        {
            const string Value = "TestValue";
            var factory = new ProjectionFactory(c => c
                .EnableSaveAssemblies()
                .GenerateSingleAssembly());

            var projection = factory.Create<IStructureType>();

            projection.AStringProperty = Value;
            var aString = projection.AStringProperty;

            projection.AnInt32Property = 42;
            var anInt32 = projection.AnInt32Property;

//            Assert.That(value, Is.SameAs(Value));
            factory.SaveGeneratedAssemblies();
        }

        [Test]
        public void TestSomeConversion()
        {
            throw new InvalidCastException();
        }

        public long Foo(object o)
        {
            var x = (object) default(DateTime);

            var y = x is Unknown ? default(DateTime) : (DateTime) x;

            return y.Ticks;
        }

        public interface IStructureType
        {
            string AStringProperty { get; set; }
            int    AnInt32Property { get; set; }
        }
    }
}
