namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    public abstract class ProjectionPropertyCollectionTests : ProjectionTestsBase
    {
        [TestFixture]
        public class Invariants
        {
            private readonly ProjectionPropertyCollection
                Properties = PropertiesOf<IAny>();

            private readonly ICollection<ProjectionProperty>
                AsCollection = PropertiesOf<IAny>();

            [Test]
            public void IsReadOnly()
            {
                Assert.That(AsCollection.IsReadOnly, Is.True);
            }

            [Test]
            public void Contains_Implicit_NullName()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => Properties.Contains(null as string)
                );
            }

            [Test]
            public void Contains_Explicit_NullName()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => Properties.Contains(null as string, typeof(IAny))
                );
            }

            [Test]
            public void Contains_Explicit_NullType()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => Properties.Contains("AnyName", null as Type)
                );
            }

            [Test]
            public void Contains_Property_Null()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => Properties.Contains(null as ProjectionProperty)
                );
            }

            [Test]
            public void Item_Implicit_NullName()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => { var _ = Properties[null as string]; }
                );
            }

            [Test]
            public void Item_Explicit_NullName()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => { var _ = Properties[null as string, typeof(IAny)]; }
                );
            }

            [Test]
            public void Item_Explicit_NullType()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => { var _ = Properties["AnyName", null as Type]; }
                );
            }

            [Test]
            public void Item_Property_NullProperty()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => { var _ = Properties[null as ProjectionProperty]; }
                );
            }

            [Test]
            public void TryGet_Implicit_NullName()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    ProjectionProperty property;
                    var _ = Properties.TryGet(null as string, out property);
                });
            }

            [Test]
            public void TryGet_Explicit_NullName()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    ProjectionProperty property;
                    var _ = Properties.TryGet(null as string, typeof(IAny), out property);
                });
            }

            [Test]
            public void TryGet_Explicit_NullType()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    ProjectionProperty property;
                    var _ = Properties.TryGet("AnyName", null as Type, out property);
                });
            }

            [Test]
            public void TryGet_Property_NullProperty()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    ProjectionProperty property = null;
                    var _ = Properties.TryGet(ref property);
                });
            }

            [Test]
            public void CopyTo_NullArray()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => Properties.CopyTo(null, 0)
                );
            }

            [Test]
            public void CopyTo_InvalidIndex()
            {
                Assert.Throws<ArgumentOutOfRangeException>
                (
                    () => Properties.CopyTo(new ProjectionProperty[0], -1)
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
                    () => new ProjectionPropertyCollection.DebugView(null)
                );
            }
        }

        [TestFixture]
        public class Empty
        {
            public interface IEmpty { }

            public interface IOther { string PropertyA { get; set; } }

            private readonly ProjectionPropertyCollection
                Properties = PropertiesOf<IEmpty>();

            private readonly ProjectionProperty
                AnyProperty = PropertiesOf<IOther>().First();

            [Test]
            public void Count()
            {
                Assert.That(Properties.Count, Is.EqualTo(0));
            }

            [Test]
            public void Contains_Implicit()
            {
                Assert.That(Properties.Contains("AnyName"), Is.False);
            }

            [Test]
            public void Contains_Explicit()
            {
                Assert.That(Properties.Contains("AnyName", typeof(IAny)), Is.False);
            }

            [Test]
            public void Contains_Property()
            {
                Assert.That(Properties.Contains(AnyProperty), Is.False);
            }

            [Test]
            public void Item_Implicit()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => { var _ = Properties["AnyName"]; }
                );
            }

            [Test]
            public void Item_Explicit()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => { var _ = Properties["AnyName", typeof(IAny)]; }
                );
            }

            [Test]
            public void Item_Property()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => { var _ = Properties[AnyProperty]; }
                );
            }

            [Test]
            public void TryGet_Implicit()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("AnyName", out property);

                Assert.That(result,   Is.False);
                Assert.That(property, Is.Null);
            }

            [Test]
            public void TryGet_Explicit()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("AnyName", typeof(IAny), out property);

                Assert.That(result,   Is.False);
                Assert.That(property, Is.Null);
            }

            [Test]
            public void TryGet_Property()
            {
                ProjectionProperty property = AnyProperty;
                var result = Properties.TryGet(ref property);

                Assert.That(result,   Is.False);
                Assert.That(property, Is.Null);
            }

            [Test]
            public void Enumerator_Generic()
            {
                Assert.That(Properties.EnumerateGeneric(), Is.Empty);
            }

            [Test]
            public void Enumerator_Nongeneric()
            {
                Assert.That(Properties.EnumerateNongeneric(), Is.Empty);
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionProperty[2];

                Properties.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array[1], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionPropertyCollection.DebugView(Properties);

                Assert.That(view.Items, Is.Empty);
            }
        }

        [TestFixture]
        public class WithDeclaredProperty
        {
            public interface IThing
            {
                string PropertyA { get; set; }
            }

            private readonly ProjectionPropertyCollection
                Properties = PropertiesOf<IThing>();

            [Test]
            public void Properties_Count()
            {
                Assert.That(Properties.Count, Is.EqualTo(1));
            }

            [Test]
            public void Contains_Implicit()
            {
                Assert.That(Properties.Contains("PropertyA"), Is.True);
            }

            [Test]
            public void Contains_Explicit()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IThing)), Is.True);
            }

            [Test]
            public void Contains_Property()
            {
                Assert.That(Properties.Contains(PropertyOf<IThing>("PropertyA")), Is.True);
            }

            [Test]
            public void Item_Implicit()
            {
                var property = Properties["PropertyA"];

                Assert.That(property, IsPropertyOf<IThing>("PropertyA"));
            }

            [Test]
            public void Item_Explicit()
            {
                var property = Properties["PropertyA", typeof(IThing)];

                Assert.That(property, IsPropertyOf<IThing>("PropertyA"));
            }

            [Test]
            public void Item_Property()
            {
                var property = Properties[PropertyOf<IThing>("PropertyA")];

                Assert.That(property, Is.SameAs(PropertyOf<IThing>("PropertyA")));
            }

            [Test]
            public void TryGet_Implicit()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IThing>("PropertyA"));
            }

            [Test]
            public void TryGet_Explicit()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IThing), out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IThing>("PropertyA"));
            }

            [Test]
            public void TryGet_Property()
            {
                ProjectionProperty property = PropertyOf<IThing>("PropertyA");
                var result = Properties.TryGet(ref property);

                Assert.That(result,   Is.True);
                Assert.That(property, Is.SameAs(PropertyOf<IThing>("PropertyA")));
            }

            [Test]
            public void Enumerator_Generic()
            {
                Assert.That(Properties.EnumerateGeneric(), Has.Count.EqualTo(1)
                    & HasPropertyOf<IThing>("PropertyA"));
            }

            [Test]
            public void Enumerator_Nongeneric()
            {
                Assert.That(Properties.EnumerateNongeneric(), Has.Count.EqualTo(1)
                    & HasPropertyOf<IThing>("PropertyA"));
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionProperty[3];

                Properties.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array[1], IsPropertyOf<IThing>("PropertyA"));
                Assert.That(array[2], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionPropertyCollection.DebugView(Properties);

                Assert.That(view.Items, Has.Length.EqualTo(1)
                    & HasPropertyOf<IThing>("PropertyA"));
            }
        }

        [TestFixture]
        public class WithUnambiguousInheritedProperty
        {
            public interface IBase
            {
                string PropertyA { get; set; }
            }

            public interface IDerived : IBase { }

            private readonly ProjectionPropertyCollection
                Properties = PropertiesOf<IDerived>();

            [Test]
            public void Properties_Count()
            {
                Assert.That(Properties.Count, Is.EqualTo(1));
            }

            [Test]
            public void Contains_Implicit()
            {
                Assert.That(Properties.Contains("PropertyA"), Is.True);
            }

            [Test]
            public void Contains_Explicit_OnDeclaringType()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IBase)), Is.True);
            }

            [Test]
            public void Contains_Explicit_OnDerivedType()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IDerived)), Is.False);
            }

            [Test]
            public void Contains_Property()
            {
                Assert.That(Properties.Contains(PropertyOf<IBase>("PropertyA")), Is.True);
            }

            [Test]
            public void Item_Implicit()
            {
                var property = Properties["PropertyA"];

                Assert.That(property, IsPropertyOf<IBase>("PropertyA"));
            }

            [Test]
            public void Item_Explicit_OnDeclaringType()
            {
                var property = Properties["PropertyA", typeof(IBase)];

                Assert.That(property, IsPropertyOf<IBase>("PropertyA"));
            }

            [Test]
            public void Item_Explicit_OnDerivedType()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => { var _ = Properties["PropertyA", typeof(IDerived)]; }
                );
            }

            [Test]
            public void Item_Property()
            {
                var property = Properties[PropertyOf<IBase>("PropertyA")];

                Assert.That(property, Is.SameAs(PropertyOf<IBase>("PropertyA")));
            }

            [Test]
            public void TryGet_Implicit()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IBase>("PropertyA"));
            }

            [Test]
            public void TryGet_Explicit_OnDeclaringType()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IBase), out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IBase>("PropertyA"));
            }

            [Test]
            public void TryGet_Explicit_OnDerivedType()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IDerived), out property);

                Assert.That(result,   Is.False);
                Assert.That(property, Is.Null);
            }

            [Test]
            public void TryGet_Property()
            {
                ProjectionProperty property = PropertyOf<IBase>("PropertyA");
                var result = Properties.TryGet(ref property);

                Assert.That(result,   Is.True);
                Assert.That(property, Is.SameAs(PropertyOf<IBase>("PropertyA")));
            }

            [Test]
            public void Enumerator_Generic()
            {
                Assert.That(Properties.EnumerateGeneric(), Has.Count.EqualTo(1)
                    & HasPropertyOf<IBase>("PropertyA"));
            }

            [Test]
            public void Enumerator_Nongeneric()
            {
                Assert.That(Properties.EnumerateNongeneric(), Has.Count.EqualTo(1)
                    & HasPropertyOf<IBase>("PropertyA"));
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionProperty[3];

                Properties.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array[1], IsPropertyOf<IBase>("PropertyA"));
                Assert.That(array[2], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionPropertyCollection.DebugView(Properties);

                Assert.That(view.Items, Has.Length.EqualTo(1)
                    & HasPropertyOf<IBase>("PropertyA"));
            }
        }

        [TestFixture]
        public class WithAmbiguousInheritedProperty
        {
            public interface IBaseA
            {
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB { }

            private readonly ProjectionPropertyCollection
                Properties = PropertiesOf<IDerived>();

            [Test]
            public void Properties_Count()
            {
                Assert.That(Properties.Count, Is.EqualTo(2));
            }

            [Test]
            public void Contains_Implicit()
            {
                Assert.That(Properties.Contains("PropertyA"), Is.False);
            }

            [Test]
            public void Contains_Explicit_OnDeclaringType()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IBaseA)), Is.True);
            }

            [Test]
            public void Contains_Explicit_OnDerivedType()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IDerived)), Is.False);
            }

            [Test]
            public void Contains_Property()
            {
                Assert.That(Properties.Contains(PropertyOf<IBaseA>("PropertyA")), Is.True);
            }

            [Test]
            public void Item_Implicit()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => { var _ = Properties["PropertyA"]; }
                );
            }

            [Test]
            public void Item_Explicit_OnDeclaringType()
            {
                var property = Properties["PropertyA", typeof(IBaseA)];

                Assert.That(property, IsPropertyOf<IBaseA>("PropertyA"));
            }

            [Test]
            public void Item_Explicit_OnDerivedType()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => { var _ = Properties["PropertyA", typeof(IDerived)]; }
                );
            }

            [Test]
            public void Item_Property()
            {
                var property = Properties[PropertyOf<IBaseA>("PropertyA")];

                Assert.That(property, Is.SameAs(PropertyOf<IBaseA>("PropertyA")));
            }

            [Test]
            public void TryGet_Implicit()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", out property);

                Assert.That(result,   Is.False);
                Assert.That(property, Is.Null);
            }

            [Test]
            public void TryGet_Explicit_OnDeclaringType()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IBaseA), out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IBaseA>("PropertyA"));
            }

            [Test]
            public void TryGet_Explicit_OnDerivedType()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IDerived), out property);

                Assert.That(result,   Is.False);
                Assert.That(property, Is.Null);
            }

            [Test]
            public void TryGet_Property()
            {
                ProjectionProperty property = PropertyOf<IBaseA>("PropertyA");
                var result = Properties.TryGet(ref property);

                Assert.That(result,   Is.True);
                Assert.That(property, Is.SameAs(PropertyOf<IBaseA>("PropertyA")));
            }

            [Test]
            public void Enumerator_Generic()
            {
                Assert.That(Properties.EnumerateGeneric(), Has.Count.EqualTo(2)
                    & HasPropertyOf<IBaseA>("PropertyA")
                    & HasPropertyOf<IBaseB>("PropertyA"));
            }

            [Test]
            public void Enumerator_Nongeneric()
            {
                Assert.That(Properties.EnumerateNongeneric(), Has.Count.EqualTo(2)
                    & HasPropertyOf<IBaseA>("PropertyA")
                    & HasPropertyOf<IBaseB>("PropertyA"));
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionProperty[4];

                Properties.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array,    HasPropertyOf<IBaseA>("PropertyA"));
                Assert.That(array,    HasPropertyOf<IBaseB>("PropertyA"));
                Assert.That(array[3], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionPropertyCollection.DebugView(Properties);

                Assert.That(view.Items, Has.Length.EqualTo(2)
                    & HasPropertyOf<IBaseA>("PropertyA")
                    & HasPropertyOf<IBaseB>("PropertyA"));
            }
        }

        [TestFixture]
        public class WithHiddenProperty
        {
            public interface IBase
            {
                string PropertyA { get; set; }
            }

            public interface IDerived : IBase
            {
                new string PropertyA { get; set; }
            }

            private readonly ProjectionPropertyCollection
                Properties = PropertiesOf<IDerived>();

            [Test]
            public void Properties_Count()
            {
                Assert.That(Properties.Count, Is.EqualTo(2));
            }

            [Test]
            public void Contains_Implicit()
            {
                Assert.That(Properties.Contains("PropertyA"), Is.True);
            }

            [Test]
            public void Contains_Explicit_OnDeclaringType()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IBase)), Is.True);
            }

            [Test]
            public void Contains_Explicit_OnDerivedType()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IDerived)), Is.True);
            }

            [Test]
            public void Contains_Property_FromDeclaringType()
            {
                Assert.That(Properties.Contains(PropertyOf<IBase>("PropertyA")), Is.True);
            }

            [Test]
            public void Contains_Property_FromDerivedType()
            {
                Assert.That(Properties.Contains(PropertyOf<IDerived>("PropertyA")), Is.True);
            }

            [Test]
            public void Item_Implicit()
            {
                var property = Properties["PropertyA"];

                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void Item_Explicit_OnDeclaringType()
            {
                var property = Properties["PropertyA", typeof(IBase)];

                Assert.That(property, IsPropertyOf<IBase>("PropertyA"));
            }

            [Test]
            public void Item_Explicit_OnDerivedType()
            {
                var property = Properties["PropertyA", typeof(IDerived)];

                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void Item_Property_FromDeclaringType()
            {
                var property = Properties[PropertyOf<IBase>("PropertyA")];

                Assert.That(property, Is.SameAs(PropertyOf<IBase>("PropertyA")));
            }

            [Test]
            public void Item_Property_FromDerivedType()
            {
                var property = Properties[PropertyOf<IDerived>("PropertyA")];

                Assert.That(property, Is.SameAs(PropertyOf<IDerived>("PropertyA")));
            }

            [Test]
            public void TryGet_Implicit()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void TryGet_Explicit_OnDeclaringType()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IBase), out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IBase>("PropertyA"));
            }

            [Test]
            public void TryGet_Explicit_OnDerivedType()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IDerived), out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void TryGet_Property_FromDeclaringType()
            {
                ProjectionProperty property = PropertyOf<IBase>("PropertyA");
                var result = Properties.TryGet(ref property);

                Assert.That(result,   Is.True);
                Assert.That(property, Is.SameAs(PropertyOf<IBase>("PropertyA")));
            }

            [Test]
            public void TryGet_Property_FromDerivedType()
            {
                ProjectionProperty property = PropertyOf<IDerived>("PropertyA");
                var result = Properties.TryGet(ref property);

                Assert.That(result,   Is.True);
                Assert.That(property, Is.SameAs(PropertyOf<IDerived>("PropertyA")));
            }

            [Test]
            public void Enumerator_Generic()
            {
                Assert.That(Properties.EnumerateGeneric(), Has.Count.EqualTo(2)
                    & HasPropertyOf<IBase   >("PropertyA")
                    & HasPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void Enumerator_Nongeneric()
            {
                Assert.That(Properties.EnumerateNongeneric(), Has.Count.EqualTo(2)
                    & HasPropertyOf<IBase   >("PropertyA")
                    & HasPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionProperty[4];

                Properties.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array,    HasPropertyOf<IBase   >("PropertyA"));
                Assert.That(array,    HasPropertyOf<IDerived>("PropertyA"));
                Assert.That(array[3], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionPropertyCollection.DebugView(Properties);

                Assert.That(view.Items, Has.Length.EqualTo(2)
                    & HasPropertyOf<IBase   >("PropertyA")
                    & HasPropertyOf<IDerived>("PropertyA"));
            }
        }

        [TestFixture]
        public class WithOverrideProperty
        {
            public interface IBase
            {
                string PropertyA { get; set; }
            }

            public interface IDerived : IBase
            {
                [Override(typeof(IBase))]
                new string PropertyA { get; set; }
            }

            private readonly ProjectionPropertyCollection
                Properties = PropertiesOf<IDerived>();

            [Test]
            public void Properties_Count()
            {
                Assert.That(Properties.Count, Is.EqualTo(1));
            }

            [Test]
            public void Contains_Implicit()
            {
                Assert.That(Properties.Contains("PropertyA"), Is.True);
            }

            [Test]
            public void Contains_Explicit_OnDeclaringType()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IBase)), Is.True);
            }

            [Test]
            public void Contains_Explicit_OnDerivedType()
            {
                Assert.That(Properties.Contains("PropertyA", typeof(IDerived)), Is.True);
            }

            [Test]
            public void Contains_Property_FromBaseType()
            {
                Assert.That(Properties.Contains(PropertyOf<IBase>("PropertyA")), Is.False);
            }

            [Test]
            public void Contains_Property_FromDerivedType()
            {
                Assert.That(Properties.Contains(PropertyOf<IDerived>("PropertyA")), Is.True);
            }

            [Test]
            public void Item_Implicit()
            {
                var property = Properties["PropertyA"];

                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void Item_Explicit_OnDeclaringType()
            {
                var property = Properties["PropertyA", typeof(IBase)];

                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void Item_Explicit_OnDerivedType()
            {
                var property = Properties["PropertyA", typeof(IDerived)];

                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void Item_Property_FromDeclaringType()
            {
                var property = Properties[PropertyOf<IBase>("PropertyA")];

                Assert.That(property, Is.SameAs(PropertyOf<IDerived>("PropertyA")));
            }

            [Test]
            public void Item_Property_FromDerivedType()
            {
                var property = Properties[PropertyOf<IDerived>("PropertyA")];

                Assert.That(property, Is.SameAs(PropertyOf<IDerived>("PropertyA")));
            }

            [Test]
            public void TryGet_Implicit()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void TryGet_Explicit_OnDeclaringType()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IBase), out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void TryGet_Explicit_OnDerivedType()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("PropertyA", typeof(IDerived), out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void TryGet_Property_FromDeclaringType()
            {
                ProjectionProperty property = PropertyOf<IBase>("PropertyA");
                var result = Properties.TryGet(ref property);

                Assert.That(result,   Is.True);
                Assert.That(property, Is.SameAs(PropertyOf<IDerived>("PropertyA")));
            }

            [Test]
            public void TryGet_Property_FromDerivedType()
            {
                ProjectionProperty property = PropertyOf<IDerived>("PropertyA");
                var result = Properties.TryGet(ref property);

                Assert.That(result,   Is.True);
                Assert.That(property, Is.SameAs(PropertyOf<IDerived>("PropertyA")));
            }

            [Test]
            public void Enumerator_Generic()
            {
                Assert.That(Properties.EnumerateGeneric(), Has.Count.EqualTo(1)
                    & HasPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void Enumerator_Nongeneric()
            {
                Assert.That(Properties.EnumerateNongeneric(), Has.Count.EqualTo(1)
                    & HasPropertyOf<IDerived>("PropertyA"));
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionProperty[3];

                Properties.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array,    HasPropertyOf<IDerived>("PropertyA"));
                Assert.That(array[2], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionPropertyCollection.DebugView(Properties);

                Assert.That(view.Items, Has.Length.EqualTo(1)
                    & HasPropertyOf<IDerived>("PropertyA"));
            }
        }

        [TestFixture]
        public class WithOverrideProperty_DifferentName
        {
            public interface IBase
            {
                string BaseProperty { get; set; }
            }

            public interface IDerived : IBase
            {
                [Override(typeof(IBase), MemberName = "BaseProperty")]
                string DerivedProperty { get; set; }
            }

            private readonly ProjectionPropertyCollection
                Properties = PropertiesOf<IDerived>();

            [Test]
            public void Properties_Count()
            {
                Assert.That(Properties.Count, Is.EqualTo(1));
            }

            [Test]
            public void Contains_Implicit_BaseName()
            {
                Assert.That(Properties.Contains("BaseProperty"), Is.True);
            }

            [Test]
            public void Contains_Implicit_DerivedName()
            {
                Assert.That(Properties.Contains("DerivedProperty"), Is.True);
            }

            [Test]
            public void Contains_Property_FromBaseType()
            {
                Assert.That(Properties.Contains(PropertyOf<IBase>("BaseProperty")), Is.False);
            }

            [Test]
            public void Contains_Property_FromDerivedType()
            {
                Assert.That(Properties.Contains(PropertyOf<IDerived>("DerivedProperty")), Is.True);
            }

            [Test]
            public void Item_Implicit_BaseName()
            {
                var property = Properties["BaseProperty"];

                Assert.That(property, IsPropertyOf<IDerived>("DerivedProperty"));
            }

            [Test]
            public void Item_Implicit_DerivedName()
            {
                var property = Properties["DerivedProperty"];

                Assert.That(property, IsPropertyOf<IDerived>("DerivedProperty"));
            }

            [Test]
            public void TryGet_Implicit_BaseName()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("BaseProperty", out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IDerived>("DerivedProperty"));
            }

            [Test]
            public void TryGet_Implicit_DerivedName()
            {
                ProjectionProperty property;
                var result = Properties.TryGet("DerivedProperty", out property);

                Assert.That(result,   Is.True);
                Assert.That(property, IsPropertyOf<IDerived>("DerivedProperty"));
            }

            [Test]
            public void Enumerator_Generic()
            {
                Assert.That(Properties.EnumerateGeneric(), Has.Count.EqualTo(1)
                    & HasPropertyOf<IDerived>("DerivedProperty"));
            }

            [Test]
            public void Enumerator_Nongeneric()
            {
                Assert.That(Properties.EnumerateNongeneric(), Has.Count.EqualTo(1)
                    & HasPropertyOf<IDerived>("DerivedProperty"));
            }

            [Test]
            public void CopyTo()
            {
                var array = new ProjectionProperty[3];

                Properties.CopyTo(array, 1);

                Assert.That(array[0], Is.Null);
                Assert.That(array,    HasPropertyOf<IDerived>("DerivedProperty"));
                Assert.That(array[2], Is.Null);
            }

            [Test]
            public void DebugView()
            {
                var view = new ProjectionPropertyCollection.DebugView(Properties);

                Assert.That(view.Items, Has.Length.EqualTo(1)
                    & HasPropertyOf<IDerived>("DerivedProperty"));
            }
        }

        private static Constraint HasPropertyOf<T>(string name)
        {
            return Has.Some.InstanceOf<ProjectionProperty>()
                .With.Name(name)
                .And.DeclaringType(TypeOf<T>());
        }

        private static Constraint IsPropertyOf<T>(string name)
        {
            return Is.InstanceOf<ProjectionProperty>()
                .With.Name(name)
                .And.DeclaringType(TypeOf<T>());
        }
    }
}
