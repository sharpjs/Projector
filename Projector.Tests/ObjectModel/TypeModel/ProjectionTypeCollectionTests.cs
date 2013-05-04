namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public abstract class ProjectionTypeCollectionTests : ProjectionTestsBase
    {
        [TestFixture]
        public class Invariants
        {
            public static readonly ProjectionTypeCollection
                BaseTypes = BaseTypesOf<IAny>();

            public static readonly ICollection<ProjectionType>
                AsCollection = BaseTypes;

            [Test]
            public void IsReadOnly()
            {
                Assert.That(AsCollection.IsReadOnly, Is.True);
            }

            [Test]
            public void Contains_ClrType_NullName()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => BaseTypes.Contains(null as Type)
                );
            }

            [Test]
            public void Contains_ProjectionType_NullType()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => BaseTypes.Contains(null as ProjectionType)
                );
            }

            [Test]
            public void Item_NullType()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => { var _ = BaseTypes[null as Type]; }
                );
            }

            [Test]
            public void TryGet_NullType()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    ProjectionType type;
                    var _ = BaseTypes.TryGet(null as Type, out type);
                });
            }

            [Test]
            public void CopyTo_NullArray()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => BaseTypes.CopyTo(null, 0)
                );
            }

            [Test]
            public void CopyTo_InvalidIndex()
            {
                Assert.Throws<ArgumentOutOfRangeException>
                (
                    () => BaseTypes.CopyTo(new ProjectionType[0], -1)
                );
            }

            [Test]
            public void Add()
            {
                Assert.Throws<NotSupportedException>(() => AsCollection.Add(null));
            }

            [Test]
            public void Remove()
            {
                Assert.Throws<NotSupportedException>(() => AsCollection.Remove(null));
            }

            [Test]
            public void Clear()
            {
                Assert.Throws<NotSupportedException>(() => AsCollection.Clear());
            }

            [Test]
            public void DebugView_NullCollection()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => new ProjectionTypeCollection.DebugView(null)
                );
            }
        }

        [TestFixture]
        public class Empty
        {
            public interface IRoot { }

            public static readonly ProjectionTypeCollection
                BaseTypes = BaseTypesOf<IRoot>();

            [Test]
            public void Count()
            {
                Assert.That(BaseTypes.Count, Is.EqualTo(0));
            }

            [Test]
            public void Contains_ClrType()
            {
                Assert.That(BaseTypes.Contains(typeof(IAny)), Is.False);
            }

            [Test]
            public void Contains_ProjectionType()
            {
                Assert.That(BaseTypes.Contains(TypeOf<IAny>()), Is.False);
            }

            [Test]
            public void Item()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => { var _ = BaseTypes[typeof(IAny)]; }
                );
            }

            [Test]
            public void TryGet()
            {
                ProjectionType type;
                var result = BaseTypes.TryGet(typeof(IAny), out type);

                Assert.That(result, Is.False);
                Assert.That(type,   Is.Null);
            }

            [Test]
            public void Enumerate_Generic()
            {
                Assert.That(BaseTypes.EnumerateGeneric(), Is.Empty);
            }

            [Test]
            public void Enumerate_Nongeneric()
            {
                Assert.That(BaseTypes.EnumerateNongeneric(), Is.Empty);
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionType[2];

                BaseTypes.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array[1], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionTypeCollection.DebugView(BaseTypes);

                Assert.That(view.Items, Is.Empty);
            }
        }

        [TestFixture]
        public class NotEmpty
        {
            public interface IBase            { }
            public interface IDerived : IBase { }

            public static readonly ProjectionTypeCollection
                BaseTypes = BaseTypesOf<IDerived>();

            public static readonly ProjectionType[]
                ExpectedBaseTypes = { TypeOf<IBase>() };

            [Test]
            public void Count()
            {
                Assert.That(BaseTypes.Count, Is.EqualTo(1));
            }

            [Test]
            public void Contains_ClrType()
            {
                Assert.That(BaseTypes.Contains(typeof(IBase)), Is.True);
            }

            [Test]
            public void Contains_ProjectionType()
            {
                Assert.That(BaseTypes.Contains(ProjectionTypeOf<IBase>()), Is.True);
            }

            [Test]
            public void Item()
            {
                Assert.That(BaseTypes[typeof(IBase)], Is.SameAs(ProjectionTypeOf<IBase>()));
            }

            [Test]
            public void TryGet()
            {
                ProjectionType type;
                var result = BaseTypes.TryGet(typeof(IBase), out type);

                Assert.That(result, Is.True);
                Assert.That(type,   Is.SameAs(ProjectionTypeOf<IBase>()));
            }

            [Test]
            public void Enumerate_Generic()
            {
                Assert.That(BaseTypes.EnumerateGeneric(), Is.EquivalentTo(ExpectedBaseTypes));
            }

            [Test]
            public void Enumerate_Nongeneric()
            {
                Assert.That(BaseTypes.EnumerateNongeneric(), Is.EquivalentTo(ExpectedBaseTypes));
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionType[3];
                
                BaseTypes.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array[1], Is.SameAs(ProjectionTypeOf<IBase>()));
                Assert.That(array[2], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionTypeCollection.DebugView(BaseTypes);

                Assert.That(view.Items, Is.EquivalentTo(ExpectedBaseTypes));
            }
        }
    }
}
