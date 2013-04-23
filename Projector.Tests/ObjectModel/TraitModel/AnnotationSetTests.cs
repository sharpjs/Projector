namespace Projector.Tests.ObjectModel
{
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using Projector.ObjectModel;

    [TestFixture]
    public class AnnotationSetTests
    {
        [Test]
        public void Initial()
        {
            var set = AnnotationSet();

            Assert.That(set,       Is.Empty);
            Assert.That(set.First, Is.Null);
        }

        [Test]
        public void Apply_One()
        {
            var a = new object();

            var set = AnnotationSet(a);

            Assert.That(set,            HasAnnotations(a));
            Assert.That(set.First,      Is.Not.Null);
            Assert.That(set.First.Item, Is.SameAs(a));
            Assert.That(set.First.Next, Is.Null);
        }

        [Test]
        public void Apply_SameInstance()
        {
            var a = new AnnotationA { };

            var set = AnnotationSet(a, a);

            Assert.That(set, HasAnnotations(a));
        }

        [Test]
        public void Apply_SameInstance_AllowMultiple()
        {
            var a = new AnnotationA { AllowMultiple = true };

            var set = AnnotationSet(a, a);

            Assert.That(set, HasAnnotations(a));
        }

        [Test]
        public void Apply_AnotherType_ThenSameInstance()
        {
            var a = new AnnotationA { };
            var b = new AnnotationB { };

            var set = AnnotationSet(a, b, a);

            // NOTE: Unlike BehaviorSet, adding same instance to AnnotationSet does NOT reorder items
            Assert.That(set, HasAnnotations(b, a));
        }

        [Test]
        public void Apply_AnotherType_ThenSameInstance_AllowMultiple()
        {
            var a = new AnnotationA { AllowMultiple = true };
            var b = new AnnotationB { AllowMultiple = true };

            var set = AnnotationSet(a, b, a);

            // NOTE: Unlike BehaviorSet, adding same instance to AnnotationSet does NOT reorder items
            Assert.That(set, HasAnnotations(b, a));
        }

        [Test]
        public void Apply_AnotherType_ThenSameType()
        {
            var a1 = new AnnotationA { };
            var b1 = new AnnotationB { };
            var a2 = new AnnotationA { };

            var set = AnnotationSet(a1, b1, a2);

            Assert.That(set, HasAnnotations(a2, b1));
        }

        [Test]
        public void Apply_AnotherType_ThenSameType_AllowMultiple()
        {
            var a1 = new AnnotationA { AllowMultiple = true };
            var b1 = new AnnotationB { AllowMultiple = true };
            var a2 = new AnnotationA { AllowMultiple = true };

            var set = AnnotationSet(a1, b1, a2);

            Assert.That(set, HasAnnotations(a2, b1, a1));
        }

        private static AnnotationSet AnnotationSet(params object[] annotations)
        {
            var set = new AnnotationSet();

            foreach (var annotation in annotations)
                set.Apply(annotation);

            return set;
        }

        private static Constraint HasAnnotations(params object[] annotations)
        {
            return Is.EqualTo(annotations);
        }
    }
}
