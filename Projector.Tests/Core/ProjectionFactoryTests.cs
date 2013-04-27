namespace Projector.Tests.Core
{
    using System;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class ProjectionFactoryTests
    {
        private readonly ProjectionFactory
            Factory = ProjectionFactory.Default;

        [Test]
        public void GetProjectionType_Once()
        {
            var type = Factory.GetProjectionType(typeof(AnyType));

            Assert.That(type,                Is.Not.Null);
            Assert.That(type.UnderlyingType, Is.SameAs(typeof(AnyType)));
        }

        [Test]
        public void GetProjectionType_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Factory.GetProjectionType(null);
            });
        }

        [Test]
        public void GetProjectionType_Again()
        {
            var typeA = Factory.GetProjectionType(typeof(AnyType));
            var typeB = Factory.GetProjectionType(typeof(AnyType));

            Assert.That(typeA, Is.SameAs(typeB));
        }

        [Test]
        public void GetProjectionType_Concurrent()
        {
            var types = new ProjectionType[ConcurrentTests.ThreadCount];

            ConcurrentTests.ParallelInvoke(i =>
            {
                types[i] = Factory.GetProjectionType(typeof(PopularType));
            });

            Assert.That(types, Has.All.SameAs(types[0]));
        }

        //[Test]
        //public void Create_Type()
        //{
        //    var projection = Factory.Create(typeof(ICreatable)) as Projection;

        //    Assert_Created(projection, ProjectionContext.Default);
        //}

        //[Test]
        //public void Create_Type_Argument()
        //{
        //    var argument   = new MockArgument();
        //    var projection = Factory.Create(typeof(ICreatable), argument) as Projection;

        //    Assert_Created(projection, ProjectionContext.Default);
        //    Assert.That(argument.WasVisited, Is.True);
        //}

        //[Test]
        //public void Create_Type_Arguments()
        //{
        //    var argumentA  = new object();
        //    var argumentB  = new MockArgument();
        //    var projection = Factory.Create(typeof(ICreatable), argumentA, argumentB) as Projection;

        //    Assert_Created(projection, ProjectionContext.Default);
        //    Assert.That(argumentB.WasVisited, Is.True);
        //}

        //[Test]
        //public void Create_Type_Context()
        //{
        //    var context    = new ProjectionContext();
        //    var projection = Factory.Create(typeof(ICreatable), context) as Projection;

        //    Assert_Created(projection, context);
        //}

        //[Test]
        //public void Create_Type_Context_Argument()
        //{
        //    var context    = new ProjectionContext();
        //    var argument   = new MockArgument();
        //    var projection = Factory.Create(typeof(ICreatable), context, argument) as Projection;

        //    Assert_Created(projection, context);
        //    Assert.That(argument.WasVisited, Is.True);
        //}

        //[Test]
        //public void Create_Type_Context_Arguments()
        //{
        //    var context    = new ProjectionContext();
        //    var argumentA  = new object();
        //    var argumentB  = new MockArgument();
        //    var projection = Factory.Create(typeof(ICreatable), context, argumentA, argumentB) as Projection;

        //    Assert_Created(projection, context);
        //    Assert.That(argumentB.WasVisited, Is.True);
        //}

        //[Test]
        //public void Create_NullType()
        //{
        //    Assert.Throws<ArgumentNullException>
        //    (
        //        () => Factory.Create(null as Type)
        //    );
        //}

        //[Test]
        //public void Create_InvalidType()
        //{
        //    Assert.Throws<ProjectionException>
        //    (
        //        () => Factory.Create(typeof(InvalidProjectionType))
        //    );
        //}

        //[Test]
        //public void Create_Type_NullContext()
        //{
        //    Assert.Throws<ArgumentNullException>
        //    (
        //        () => Factory.Create(typeof(ICreatable), null as IProjectionContext)
        //    );
        //}

        //[Test]
        //public void Create_Generic()
        //{
        //    var projection = Factory.Create<ICreatable>() as Projection;

        //    Assert_Created(projection, ProjectionContext.Default);
        //}

        //[Test]
        //public void Create_Generic_Argument()
        //{
        //    var argument   = new MockArgument();
        //    var projection = Factory.Create<ICreatable>(argument) as Projection;

        //    Assert_Created(projection, ProjectionContext.Default);
        //    Assert.That(argument.WasVisited, Is.True);
        //}

        //[Test]
        //public void Create_Generic_Arguments()
        //{
        //    var argumentA  = new object();
        //    var argumentB  = new MockArgument();
        //    var projection = Factory.Create<ICreatable>(argumentA, argumentB) as Projection;

        //    Assert_Created(projection, ProjectionContext.Default);
        //    Assert.That(argumentB.WasVisited, Is.True);
        //}

        //[Test]
        //public void Create_Generic_Context()
        //{
        //    var context    = new ProjectionContext();
        //    var projection = Factory.Create<ICreatable>(context) as Projection;

        //    Assert_Created(projection, context);
        //}

        //[Test]
        //public void Create_Generic_Context_Argument()
        //{
        //    var context    = new ProjectionContext();
        //    var argument   = new MockArgument();
        //    var projection = Factory.Create<ICreatable>(context, argument) as Projection;

        //    Assert_Created(projection, context);
        //    Assert.That(argument.WasVisited, Is.True);
        //}

        //[Test]
        //public void Create_Generic_Context_Arguments()
        //{
        //    var context    = new ProjectionContext();
        //    var argumentA  = new object();
        //    var argumentB  = new MockArgument();
        //    var projection = Factory.Create<ICreatable>(context, argumentA, argumentB) as Projection;

        //    Assert_Created(projection, context);
        //    Assert.That(argumentB.WasVisited, Is.True);
        //}

        //[Test]
        //public void Create_Generic_InvalidType()
        //{
        //    Assert.Throws<ProjectionException>
        //    (
        //        () => Factory.Create<InvalidProjectionType>()
        //    );
        //}

        //[Test]
        //public void Create_Generic_NullContext()
        //{
        //    Assert.Throws<ArgumentNullException>
        //    (
        //        () => Factory.Create<ICreatable>(null as IProjectionContext)
        //    );
        //}

        public sealed class AnyType     { }
        public sealed class PopularType { }

        //[ArgumentAware]
        //public interface ICreatable { }

        //[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
        //public sealed class ArgumentAwareAttribute : ProjectionBehaviorAttribute, IInstanceInitializer
        //{
        //    public void InitializeInstance(InstanceInitializerInvocation invocation)
        //    {
        //        var arg = invocation.GetArgument<MockArgument>();
        //        if (arg != null)
        //            arg.WasVisited = true;
        //    }
        //}

        //private sealed class MockArgument
        //{
        //    public bool WasVisited { get; set; }
        //}

        //private void Assert_Created(Projection projection, IProjectionContext context)
        //{
        //    Assert.That(projection,                  Is.Not.Null & Is.InstanceOf<ICreatable>());
        //    Assert.That(projection.Type,             Is.SameAs(Factory.GetProjectionType(typeof(ICreatable))));
        //    Assert.That(projection.Instance,         Is.Not.Null);
        //    Assert.That(projection.Instance.Context, Is.SameAs(context));
        //}
    }
}
