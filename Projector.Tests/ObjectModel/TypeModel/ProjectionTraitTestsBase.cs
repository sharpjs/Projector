namespace Projector.ObjectModel
{
    using System.Linq;
    using NUnit.Framework;

    public abstract class ProjectionTraitTestsBase : ProjectionTestsBase
    {
        public abstract class TraitsCase
        {
            protected abstract ProjectionMetaObject GetTarget();
        }

        public abstract class TraitsCase_Normal : TraitsCase
        {
            protected ProjectionMetaObject Target { get; private set; }

            public TraitsCase_Normal()
            {
                Target = GetTarget();
            }
        }

        public abstract class TraitsCase_Exception : TraitsCase
        {
            [Test]
            public void Construct()
            {
                Assert.Throws<ProjectionException>
                (
                    () => GetTarget()
                );
            }
        }

        public abstract class TraitsCase_Empty : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits, Is.Empty);
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors, Is.Empty);
            }
        }

        public abstract class TraitsCase_HasOne : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits.ToList(), Has.Count.EqualTo(1)
                    & Has.Some.InstanceOf<AnnotationAAttribute>()
                );
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors.ToList(), Has.Count.EqualTo(1)
                    & Has.Some.InstanceOf<BehaviorAAttribute>()
                );
            }
        }

        public abstract class TraitsCase_HasMultiple : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits.ToList(), Has.Count.EqualTo(2)
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("1")
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("2")
                );
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors.ToList(), Has.Count.EqualTo(2)
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("1")
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("2")
                );
            }
        }

        public abstract class TraitsCase_HasBase : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits.ToList(), Has.Count.EqualTo(2)
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("BaseB")
                );
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors.ToList(), Has.Count.EqualTo(2)
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("BaseB")
                );
            }
        }

        public abstract class TraitsCase_HasDerived : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits.ToList(), Has.Count.EqualTo(1)
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("Derived")
                );
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors.ToList(), Has.Count.EqualTo(1)
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("Derived")
                );
            }
        }

        public abstract class TraitsCase_HasBaseAndDerived : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits.ToList(), Has.Count.EqualTo(3)
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("BaseB")
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("Derived")
                );
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors.ToList(), Has.Count.EqualTo(3)
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("BaseB")
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("Derived")
                );
            }
        }

        public abstract class TraitsCase_WithNoTraits                                       : TraitsCase_Empty       { }
        public abstract class TraitsCase_WithNonInheritableTraits                           : TraitsCase_Empty       { }
        public abstract class TraitsCase_WithDeclaredTraits_AllowSingle                     : TraitsCase_HasOne      { }
        public abstract class TraitsCase_WithDeclaredTraits_AllowMultiple                   : TraitsCase_HasMultiple { }
        public abstract class TraitsCase_WithInheritedTraits_AllowSingle                    : TraitsCase_HasOne      { }
        public abstract class TraitsCase_WithInheritedTraits_AllowMultiple                  : TraitsCase_HasMultiple { }
        public abstract class TraitsCase_WithInheritedTraits_Diamond_AllowSingle            : TraitsCase_HasOne      { }
        public abstract class TraitsCase_WithInheritedTraits_Diamond_AllowMultiple          : TraitsCase_HasOne      { }
        public abstract class TraitsCase_WithInheritedTraits_Multiple_AllowSingle           : TraitsCase_Exception   { }
        public abstract class TraitsCase_WithInheritedTraits_Multiple_AllowMultiple         : TraitsCase_HasBase     { }
        public abstract class TraitsCase_WithInheritedTraits_Override_AllowSingle           : TraitsCase_HasDerived  { }
        public abstract class TraitsCase_WithInheritedTraits_Override_AllowMultiple         : TraitsCase_HasBaseAndDerived { }
        public abstract class TraitsCase_WithInheritedTraits_Override_Inherit_AllowSingle   : TraitsCase_HasDerived  { }
        public abstract class TraitsCase_WithInheritedTraits_Override_Inherit_AllowMultiple : TraitsCase_HasBaseAndDerived { }

        public abstract class TraitsCase_WithInheritDirective_DefaultSource : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits.ToList(), Has.Count.EqualTo(2)
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("BaseB")
                );
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors.ToList(), Has.Count.EqualTo(2)
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("BaseB")
                );
            }
        }

        public abstract class TraitsCase_WithInheritDirective_ExplicitSource : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits.ToList(), Has.Count.EqualTo(4)
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<AnnotationBAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<AnnotationBAttribute>().With.Tag("BaseB")
                    & Has.Some.InstanceOf<AnnotationBAttribute>().With.Tag("BaseC")
                );
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors.ToList(), Has.Count.EqualTo(4)
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("BaseB")
                    & Has.Some.InstanceOf<BehaviorBAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<BehaviorBAttribute>().With.Tag("BaseB")
                    & Has.Some.InstanceOf<BehaviorBAttribute>().With.Tag("BaseC")
                );
            }
        }

        public abstract class TraitsCase_WithInheritDirective_ExplicitAndDefaultSources : TraitsCase_Normal
        {
            [Test]
            public void Annotations()
            {
                Assert.That(Target.Traits.ToList(), Has.Count.EqualTo(3)
                    & Has.Some.InstanceOf<AnnotationAAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<AnnotationBAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<AnnotationBAttribute>().With.Tag("BaseB")
                );
            }

            [Test]
            public void Behaviors()
            {
                Assert.That(Target.Behaviors.ToList(), Has.Count.EqualTo(3)
                    & Has.Some.InstanceOf<BehaviorAAttribute>().With.Tag("BaseB")
                    & Has.Some.InstanceOf<BehaviorBAttribute>().With.Tag("BaseA")
                    & Has.Some.InstanceOf<BehaviorBAttribute>().With.Tag("BaseB")
                );
            }
        }

        public abstract class TraitsCase_WithSuppressDirective : TraitsCase_HasOne { }
        public abstract class TraitsCase_WithSuppressDirective_AndInheritDirective : TraitsCase_HasOne { }
    }
}
