namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public abstract class ProjectionStructureTypeTests : ProjectionTraitTestsBase
    {
        [TestFixture]
        public class Invariants
        {
            private interface IAny { }

            private readonly ProjectionType Type = TypeOf<IAny>();

            [Test]
            public void Metatype()
            {
                Assert.That(Type, Is.InstanceOf<ProjectionStructureType>()); ;
            }

            [Test]
            public void Kind()
            {
                Assert.That(Type.Kind, Is.EqualTo(TypeKind.Structure));
            }

            [Test]
            public void IsVirtualizable()
            {
                Assert.That(Type.IsVirtualizable, Is.True);
            }

            [Test]
            public void CollectionKeyType()
            {
                Assert.That(Type.CollectionKeyType, Is.Null);
            }

            [Test]
            public void CollectionItemType()
            {
                Assert.That(Type.CollectionItemType, Is.Null);
            }
        }

        [TestFixture]
        public class WithInvalidBaseType
        {
            public interface IAny { }
            public interface IInvalid : IList<IAny> { }

            [Test]
            public void Construct()
            {
                Assert.Throws<ProjectionException>
                (
                    () => TypeOf<IInvalid>()
                );
            }
        }

        [TestFixture]
        public class WithNoBaseTypes
        {
            public interface IRoot { }

            [Test]
            public void BaseTypes()
            {
                Assert.That(TypeOf<IRoot>().BaseTypes, Is.Empty);
            }
        }

        [TestFixture]
        public class WithBaseTypes
        {
            private interface IBaseX                    { }
            private interface IBaseA                    { }
            private interface IBaseB   : IBaseX         { }
            private interface IDerived : IBaseA, IBaseB { }

            private readonly ProjectionType Type = TypeOf<IDerived>();

            [Test]
            public void BaseTypes()
            {
                Assert.That(Type.BaseTypes, Is.EquivalentTo(new[] { TypeOf<IBaseA>(), TypeOf<IBaseB>() }));
            }
        }

        [TestFixture]
        public class WithNoTraits : TraitsCase_WithNoTraits
        {
            public interface IEmpty { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IEmpty>();
            }
        }

        [TestFixture]
        public class WithDeclaredTraits_AllowSingle : TraitsCase_WithDeclaredTraits_AllowSingle
        {
            [AnnotationA(AllowMultiple = false)]
            [BehaviorA  (AllowMultiple = false)]
            public interface IFoo { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IFoo>();
            }
        }

        [TestFixture]
        public class WithDeclaredTraits_AllowMultiple : TraitsCase_WithDeclaredTraits_AllowMultiple
        {
            [AnnotationA(AllowMultiple = true, Tag="1")]
            [AnnotationA(AllowMultiple = true, Tag="2")]
            [BehaviorA  (AllowMultiple = true, Tag="1")]
            [BehaviorA  (AllowMultiple = true, Tag="2")]
            public interface IFoo { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IFoo>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_AllowSingle : TraitsCase_WithInheritedTraits_AllowSingle
        {
            [AnnotationA(AllowMultiple = false, Inherited = true)]
            [BehaviorA  (AllowMultiple = false, Inherited = true)]
            public interface IBase { }

            public interface IDerived : IBase { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_AllowMultiple : TraitsCase_WithInheritedTraits_AllowMultiple
        {
            [AnnotationA(AllowMultiple = true, Inherited = true, Tag="1")]
            [AnnotationA(AllowMultiple = true, Inherited = true, Tag="2")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag="1")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag="2")]
            public interface IBase { }

            public interface IDerived : IBase { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithNonInheritableTraits : TraitsCase_WithNonInheritableTraits
        {
            [AnnotationA(Inherited = false)]
            [BehaviorA  (Inherited = false)]
            public interface IBase { }

            public interface IDerived : IBase { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Diamond_AllowSingle
        : TraitsCase_WithInheritedTraits_Diamond_AllowSingle
        {
            [AnnotationA(AllowMultiple = false, Inherited = true)]
            [BehaviorA  (AllowMultiple = false, Inherited = true)]
            public interface IBase { }

            public interface IBaseA : IBase { }
            public interface IBaseB : IBase { }

            public interface IDerived : IBaseA, IBaseB { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Diamond_AllowMultiple
        : TraitsCase_WithInheritedTraits_Diamond_AllowMultiple
        {
            [AnnotationA(AllowMultiple = true, Inherited = true)]
            [BehaviorA  (AllowMultiple = true, Inherited = true)]
            public interface IBase { }

            public interface IBaseA : IBase { }
            public interface IBaseB : IBase { }

            public interface IDerived : IBaseA, IBaseB { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Multiple_AllowSingle
        : TraitsCase_WithInheritedTraits_Multiple_AllowSingle
        {
            [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "1")]
            [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "1")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "2")]
            [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "2")]
            public interface IBaseB { }

            public interface IDerived : IBaseA, IBaseB { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Multiple_AllowMultiple
        : TraitsCase_WithInheritedTraits_Multiple_AllowMultiple
        {
            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            public interface IBaseB { }

            public interface IDerived : IBaseA, IBaseB { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Override_AllowSingle
        : TraitsCase_WithInheritedTraits_Override_AllowSingle
        {
            [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "BaseA")]
            [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "BaseA")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "BaseB")]
            [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "BaseB")]
            public interface IBaseB { }

            [AnnotationA(AllowMultiple = false, Tag = "Derived")]
            [BehaviorA  (AllowMultiple = false, Tag = "Derived")]
            public interface IDerived : IBaseA, IBaseB { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Override_AllowMultiple
        : TraitsCase_WithInheritedTraits_Override_AllowMultiple
        {
            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            public interface IBaseB { }

            [AnnotationA(AllowMultiple = true, Tag = "Derived")]
            [BehaviorA  (AllowMultiple = true, Tag = "Derived")]
            public interface IDerived : IBaseA, IBaseB { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Override_Inherit_AllowSingle
        : TraitsCase_WithInheritedTraits_Override_Inherit_AllowSingle
        {
            [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "BaseA")]
            [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "BaseA")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "BaseB")]
            [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "BaseB")]
            public interface IBaseB { }

            [AnnotationA(AllowMultiple = false, Inherited = true, Tag = "Derived")]
            [BehaviorA  (AllowMultiple = false, Inherited = true, Tag = "Derived")]
            public interface IDerived : IBaseA, IBaseB { }

            public interface IMoreDerived : IDerived { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IMoreDerived>();
            }
        }

        [TestFixture]
        public class WithInheritedTraits_Override_Inherit_AllowMultiple
        : TraitsCase_WithInheritedTraits_Override_Inherit_AllowMultiple
        {
            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            public interface IBaseB { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "Derived")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "Derived")]
            public interface IDerived : IBaseA, IBaseB { }

            public interface IMoreDerived : IDerived { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IMoreDerived>();
            }
        }

        [TestFixture]
        public class WithInheritDirective_DefaultSource
        : TraitsCase_WithInheritDirective_DefaultSource
        {
            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            public interface IBaseB { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            public interface IBaseC { }

            // Inherit traits from only IBaseA + IBaseB
            [InheritFrom(typeof(IBaseA))]
            [InheritFrom(typeof(IBaseB))]
            public interface IDerived : IBaseA, IBaseB, IBaseC { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritDirective_ExplicitSource
        : TraitsCase_WithInheritDirective_ExplicitSource
        {
            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            public interface IBaseB { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            public interface IBaseC { }

            // Inherit {Annotation|Behavior}A from IBaseA
            // Inherit other traits from all bases
            [InheritFrom(typeof(IBaseA), AttributeType = typeof(AnnotationAAttribute))]
            [InheritFrom(typeof(IBaseB), AttributeType = typeof(BehaviorAAttribute  ))]
            public interface IDerived : IBaseA, IBaseB, IBaseC { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithInheritDirective_ExplicitAndDefaultSources
        : TraitsCase_WithInheritDirective_ExplicitAndDefaultSources
        {
            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseA")]
            public interface IBaseA { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseB")]
            public interface IBaseB { }

            [AnnotationA(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            [AnnotationB(AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            [BehaviorA  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            [BehaviorB  (AllowMultiple = true, Inherited = true, Tag = "BaseC")]
            public interface IBaseC { }

            // Inherit AnnotationA from IBaseA
            // Inherit other traits from IBaseA + IBaseB
            [InheritFrom(typeof(IBaseA), AttributeType = typeof(AnnotationAAttribute))]
            [InheritFrom(typeof(IBaseB), AttributeType = typeof(BehaviorAAttribute))]
            [InheritFrom(typeof(IBaseA))]
            [InheritFrom(typeof(IBaseB))]
            public interface IDerived : IBaseA, IBaseB, IBaseC { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithSuppressDirective
        : TraitsCase_WithSuppressDirective
        {
            [AnnotationA(AllowMultiple = true, Inherited = true)]
            [AnnotationB(AllowMultiple = true, Inherited = true)]
            [BehaviorA  (AllowMultiple = true, Inherited = true)]
            [BehaviorB  (AllowMultiple = true, Inherited = true)]
            public interface IBase { }

            [Suppress(typeof(AnnotationBAttribute))]
            [Suppress(typeof(BehaviorBAttribute  ))]
            public interface IDerived : IBase { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }

        [TestFixture]
        public class WithSuppressDirective_AndInheritDirective
        : TraitsCase_WithSuppressDirective_AndInheritDirective
        {
            [AnnotationA(AllowMultiple = true, Inherited = true)]
            [AnnotationB(AllowMultiple = true, Inherited = true)]
            [BehaviorA  (AllowMultiple = true, Inherited = true)]
            [BehaviorB  (AllowMultiple = true, Inherited = true)]
            public interface IBase { }

            [Suppress(typeof(AnnotationBAttribute))] // takes precedence over [InheritFrom]
            [Suppress(typeof(BehaviorBAttribute  ))] // takes precedence over [InheritFrom]
            [InheritFrom(typeof(IBase), AttributeType = typeof(AnnotationAAttribute))] // no effect
            [InheritFrom(typeof(IBase), AttributeType = typeof(BehaviorAAttribute  ))] // no effect
            public interface IDerived : IBase { }

            protected override ProjectionMetaObject GetTarget()
            {
                return TypeOf<IDerived>();
            }
        }
    }
}
