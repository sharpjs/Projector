namespace Projector.Tests.ObjectModel
{
    using System.Reflection;
    using NUnit.Framework;
    using Projector.ObjectModel;

    public sealed class ProjectionPropertyTests : ProjectionTraitTestsBase
    {
        private ProjectionPropertyTests() { }

        [TestFixture]
        public class Invariants
        {
            public interface IFoo
            {
                string PropertyA { get; set; }
            }

            private readonly ProjectionProperty
                Property = PropertyOf<IFoo>("PropertyA");

            [Test]
            public void ToString_()
            {
                Assert.That(Property.ToString(), Is.Not.Null);
            }
        }

        [TestFixture]
        public class ForNormalProperty // Declared, Read/Write
        {
            public interface IFoo
            {
                string PropertyA { get; set; }
            }

            private readonly ProjectionProperty
                Property = PropertyOf<IFoo>("PropertyA");

            [Test]
            public void Name()
            {
                Assert.That(Property.Name, Is.EqualTo("PropertyA"));
            }

            [Test]
            public void DeclaringType()
            {
                Assert.That(Property.DeclaringType, Is.SameAs(ProjectionTypeOf<IFoo>()));
            }

            [Test]
            public void PropertyType()
            {
                Assert.That(Property.PropertyType, Is.SameAs(TypeOf<string>()));
            }

            [Test]
            public void Overrides()
            {
                Assert.That(Property.Overrides, Is.Empty);
            }

            [Test]
            public void CanRead()
            {
                Assert.That(Property.CanRead, Is.True);
            }

            [Test]
            public void CanWrite()
            {
                Assert.That(Property.CanWrite, Is.True);
            }

            [Test]
            public void UnderlyingGetter()
            {
                Assert.That(Property.UnderlyingGetter,
                    Is.SameAs(typeof(IFoo).GetProperty("PropertyA").GetGetMethod()));
            }

            [Test]
            public void UnderlyingSetter()
            {
                Assert.That(Property.UnderlyingSetter,
                    Is.SameAs(typeof(IFoo).GetProperty("PropertyA").GetSetMethod()));
            }
        }

        [TestFixture]
        public class ForReadOnlyProperty
        {
            public interface IFoo
            {
                string PropertyA { get; }
            }

            private readonly ProjectionProperty
                Property = PropertyOf<IFoo>("PropertyA");


            [Test]
            public void CanRead()
            {
                Assert.That(Property.CanRead, Is.True);
            }

            [Test]
            public void CanWrite()
            {
                Assert.That(Property.CanWrite, Is.False);
            }

            [Test]
            public void UnderlyingGetter()
            {
                Assert.That(Property.UnderlyingGetter,
                    Is.SameAs(typeof(IFoo).GetProperty("PropertyA").GetGetMethod()));
            }

            [Test]
            public void UnderlyingSetter()
            {
                Assert.That(Property.UnderlyingSetter, Is.Null);
            }
        }

        [TestFixture]
        public class ForWriteOnlyProperty
        {
            public interface IFoo
            {
                string PropertyA { set; }
            }

            private readonly ProjectionProperty
                Property = PropertyOf<IFoo>("PropertyA");

            [Test]
            public void CanRead()
            {
                Assert.That(Property.CanRead, Is.False);
            }

            [Test]
            public void CanWrite()
            {
                Assert.That(Property.CanWrite, Is.True);
            }

            [Test]
            public void UnderlyingGetter()
            {
                Assert.That(Property.UnderlyingGetter, Is.Null);
            }

            [Test]
            public void UnderlyingSetter()
            {
                Assert.That(Property.UnderlyingSetter, 
                    Is.SameAs(typeof(IFoo).GetProperty("PropertyA").GetSetMethod()));
            }
        }

        [TestFixture]
        public class ForPropertyInGenericType
        {
            public interface IGeneric<T>
            {
                T PropertyA { get; set; }
            }

            private readonly ProjectionProperty
                Property = PropertyOf<IGeneric<string>>("PropertyA");

            [Test]
            public void CanRead()
            {
                Assert.That(Property.CanRead, Is.True);
            }

            [Test]
            public void CanWrite()
            {
                Assert.That(Property.CanWrite, Is.True);
            }

            [Test]
            public void UnderlyingGetter()
            {
                Assert.That(Property.UnderlyingGetter,
                    Is.SameAs(typeof(IGeneric<string>).GetProperty("PropertyA").GetGetMethod()));
            }

            [Test]
            public void UnderlyingSetter()
            {
                Assert.That(Property.UnderlyingSetter,
                    Is.SameAs(typeof(IGeneric<string>).GetProperty("PropertyA").GetSetMethod()));
            }
        }

        [TestFixture]
        public class ForOverrideProperty
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

            private readonly ProjectionProperty
                Property = PropertyOf<IDerived>("PropertyA");

            [Test]
            public void Name()
            {
                Assert.That(Property.Name, Is.EqualTo("PropertyA"));
            }

            [Test]
            public void DeclaringType()
            {
                Assert.That(Property.DeclaringType, Is.SameAs(ProjectionTypeOf<IDerived>()));
            }

            [Test]
            public void PropertyType()
            {
                Assert.That(Property.PropertyType, Is.SameAs(TypeOf<string>()));
            }

            [Test]
            public void Overrides()
            {
                Assert.That(Property.Overrides, Has.Count.EqualTo(1)
                    & Has.Some.InstanceOf<ProjectionProperty>()
                        .With.Name("PropertyA")
                        .And.DeclaringType(TypeOf<IBase>())
                );
            }
        }

        [TestFixture]
        public class ForOverrideProperty_DifferentName
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

            private readonly ProjectionProperty
                Property = PropertyOf<IDerived>("DerivedProperty");

            [Test]
            public void Name()
            {
                Assert.That(Property.Name, Is.EqualTo("DerivedProperty"));
            }

            [Test]
            public void DeclaringType()
            {
                Assert.That(Property.DeclaringType, Is.SameAs(ProjectionTypeOf<IDerived>()));
            }

            [Test]
            public void PropertyType()
            {
                Assert.That(Property.PropertyType, Is.SameAs(TypeOf<string>()));
            }

            [Test]
            public void Overrides()
            {
                Assert.That(Property.Overrides, Has.Count.EqualTo(1)
                    & Has.Some.InstanceOf<ProjectionProperty>()
                        .With.Name("BaseProperty")
                        .And.DeclaringType(TypeOf<IBase>())
                );
            }
        }

        [TestFixture]
        public class ForOverrideProperty_Invalid
        {
            public interface INotBase
            {
                string PropertyA { get; set; }
            }

            public interface INotDerived
            {
                [Override(typeof(INotBase))]
                string PropertyA { get; set; }
            }

            [Test]
            public void Name()
            {
                Assert.Throws<ProjectionException>
                (
                    () => PropertyOf<INotDerived>("PropertyA")
                );
            }
        }

        [TestFixture]
        public class WithNoTraits
        : TraitsCase_WithNoTraits
        {
            public interface IFoo
            {
                string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IFoo>("PropertyA");
            }
        }

        [TestFixture]
        public class WithDeclaredTraits_AllowSingle
        : TraitsCase_WithDeclaredTraits_AllowSingle
        {
            public interface IFoo
            {
                [AnnotationA(AllowMultiple = false)]
                [BehaviorA  (AllowMultiple = false)]
                string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IFoo>("PropertyA");
            }
        }

        [TestFixture]
        public class WithDeclaredTraits_AllowMultiple
        : TraitsCase_WithDeclaredTraits_AllowMultiple
        {
            public interface IFoo
            {
                [AnnotationA(AllowMultiple = true, Tag="1")]
                [AnnotationA(AllowMultiple = true, Tag="2")]
                [BehaviorA  (AllowMultiple = true, Tag="1")]
                [BehaviorA  (AllowMultiple = true, Tag="2")]
                string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IFoo>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_AllowSingle
        : TraitsCase_WithInheritedTraits_AllowSingle
        {
            public interface IBase
            {
                [AnnotationA(AllowMultiple = false, Inherited = true)]
                [BehaviorA  (AllowMultiple = false, Inherited = true)]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBase
            {
                [Override(typeof(IBase))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_AllowMultiple
        : TraitsCase_WithInheritedTraits_AllowMultiple
        {
            public interface IBase
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag="1")]
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag="2")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag="1")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag="2")]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBase
            {
                [Override(typeof(IBase))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithNonInheritableTraits
        : TraitsCase_WithNonInheritableTraits
        {
            public interface IBase
            {
                [AnnotationA(Inherited = false)]
                [BehaviorA  (Inherited = false)]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBase
            {
                [Override(typeof(IBase))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Diamond_AllowSingle
        : TraitsCase_WithInheritedTraits_Diamond_AllowSingle
        {
            public interface IBase
            {
                [AnnotationA(AllowMultiple = false, Inherited = true)]
                [BehaviorA  (AllowMultiple = false, Inherited = true)]
                string PropertyA { get; set; }
            }

            public interface IBaseA : IBase
            {
                [Override(typeof(IBase))]
                new string PropertyA { get; set; }
            }

            public interface IBaseB : IBase
            {
                [Override(typeof(IBase))]
                new string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Diamond_AllowMultiple
        : TraitsCase_WithInheritedTraits_Diamond_AllowMultiple
        {
            public interface IBase
            {
                [AnnotationA(AllowMultiple = true, Inherited = true)]
                [BehaviorA  (AllowMultiple = true, Inherited = true)]
                string PropertyA { get; set; }
            }

            public interface IBaseA : IBase
            {
                [Override(typeof(IBase))]
                new string PropertyA { get; set; }
            }

            public interface IBaseB : IBase
            {
                [Override(typeof(IBase))]
                new string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Multiple_AllowSingle
        : TraitsCase_WithInheritedTraits_Multiple_AllowSingle
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "1")]
                [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "1")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "2")]
                [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "2")]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Multiple_AllowMultiple
        : TraitsCase_WithInheritedTraits_Multiple_AllowMultiple
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Override_AllowSingle
        : TraitsCase_WithInheritedTraits_Override_AllowSingle
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "BaseA")]
                [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "BaseA")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "BaseB")]
                [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "BaseB")]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "Derived")]
                [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "Derived")]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Override_AllowMultiple
        : TraitsCase_WithInheritedTraits_Override_AllowMultiple
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "Derived")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "Derived")]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Override_Inherit_AllowSingle
        : TraitsCase_WithInheritedTraits_Override_Inherit_AllowSingle
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "BaseA")]
                [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "BaseA")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "BaseB")]
                [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "BaseB")]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "Derived")]
                [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "Derived")]
                new string PropertyA { get; set; }
            }

            public interface IMoreDerived : IDerived
            {
                [Override(typeof(IDerived))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IMoreDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Override_Inherit_AllowMultiple
        : TraitsCase_WithInheritedTraits_Override_Inherit_AllowMultiple
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBaseA, IBaseB
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "Derived")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "Derived")]
                new string PropertyA { get; set; }
            }

            public interface IMoreDerived : IDerived
            {
                [Override(typeof(IDerived))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IMoreDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritDirective_DefaultSource
        : TraitsCase_WithInheritDirective_DefaultSource
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                string PropertyA { get; set; }
            }

            public interface IBaseC
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                string PropertyA { get; set; }
            }

            // Inherit traits from only IBaseA + IBaseB
            public interface IDerived : IBaseA, IBaseB, IBaseC
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                [Override(typeof(IBaseC))]
                [InheritFrom(typeof(IBaseA))]
                [InheritFrom(typeof(IBaseB))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritDirective_ExplicitSource
        : TraitsCase_WithInheritDirective_ExplicitSource
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                string PropertyA { get; set; }
            }

            public interface IBaseC
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                string PropertyA { get; set; }
            }

            // Inherit {Annotation|Behavior}A from IBaseA
            // Inherit other traits from all bases
            public interface IDerived : IBaseA, IBaseB, IBaseC
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                [Override(typeof(IBaseC))]
                [InheritFrom(typeof(IBaseA), AttributeType = typeof(AnnotationAAttribute))]
                [InheritFrom(typeof(IBaseB), AttributeType = typeof(BehaviorAAttribute  ))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithInheritDirective_ExplicitAndDefaultSources
        : TraitsCase_WithInheritDirective_ExplicitAndDefaultSources
        {
            public interface IBaseA
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
                string PropertyA { get; set; }
            }

            public interface IBaseB
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
                string PropertyA { get; set; }
            }

            public interface IBaseC
            {
                [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
                string PropertyA { get; set; }
            }

            // Inherit AnnotationA from IBaseA
            // Inherit other traits from IBaseA + IBaseB
            public interface IDerived : IBaseA, IBaseB, IBaseC
            {
                [Override(typeof(IBaseA))]
                [Override(typeof(IBaseB))]
                [Override(typeof(IBaseC))]
                [InheritFrom(typeof(IBaseA), AttributeType = typeof(AnnotationAAttribute))]
                [InheritFrom(typeof(IBaseB), AttributeType = typeof(BehaviorAAttribute))]
                [InheritFrom(typeof(IBaseA))]
                [InheritFrom(typeof(IBaseB))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithSuppressDirective
        : TraitsCase_WithSuppressDirective
        {
            public interface IBase
            {
                [AnnotationA(AllowMultiple = true, Inherited = true)]
                [AnnotationB(AllowMultiple = true, Inherited = true)]
                [BehaviorA  (AllowMultiple = true, Inherited = true)]
                [BehaviorB  (AllowMultiple = true, Inherited = true)]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBase
            {
                [Override(typeof(IBase))]
                [Suppress(typeof(AnnotationBAttribute))]
                [Suppress(typeof(BehaviorBAttribute  ))]
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }

        [TestFixture]
        public class WithSuppressDirective_AndInheritDirective
        : TraitsCase_WithSuppressDirective_AndInheritDirective
        {
            public interface IBase
            {
                [AnnotationA(AllowMultiple = true, Inherited = true)]
                [AnnotationB(AllowMultiple = true, Inherited = true)]
                [BehaviorA  (AllowMultiple = true, Inherited = true)]
                [BehaviorB  (AllowMultiple = true, Inherited = true)]
                string PropertyA { get; set; }
            }

            public interface IDerived : IBase
            {
                [Override(typeof(IBase))]
                [Suppress(typeof(AnnotationBAttribute))] // takes precedence over [InheritFrom]
                [Suppress(typeof(BehaviorBAttribute  ))] // takes precedence over [InheritFrom]
                [InheritFrom(typeof(IBase), AttributeType = typeof(AnnotationAAttribute))] // no effect
                [InheritFrom(typeof(IBase), AttributeType = typeof(BehaviorAAttribute  ))] // no effect
                new string PropertyA { get; set; }
            }

            protected override ProjectionMetaObject GetTarget()
            {
                return PropertyOf<IDerived>("PropertyA");
            }
        }
    }
}
