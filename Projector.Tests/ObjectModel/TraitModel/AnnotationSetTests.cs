namespace Projector.Tests.ObjectModel
{
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class AnnotationSetTests
    {
        [Test]
        public void Initially()
        {
            var annotations = new AnnotationSet();

            Assert.That(annotations,       Is.Empty);
            Assert.That(annotations.First, Is.Null);
        }

        [Test]
        public void AddFirst()
        {
            var annotations = new[] { new object() };

            var set = new AnnotationSet();
            set.Apply(annotations[0]);

            Assert.That(set,            Is.EqualTo(annotations));
            Assert.That(set.First,      Is.Not.Null);
            Assert.That(set.First.Item, Is.SameAs(annotations[0]));
            Assert.That(set.First.Next, Is.Null);
        }

        [Test]
        public void AddAnother()
        {
            var annotations = new[]
            {
                new object(),
                new AnnotationA { },
                new AnnotationB { }
            };

            var set = new AnnotationSet();
            set.Apply(annotations[2]);
            set.Apply(annotations[1]);
            set.Apply(annotations[0]);

            Assert.That(set,                      Is.EquivalentTo(annotations));
            Assert.That(set.First,                Is.Not.Null);
            Assert.That(set.First.Item,           Is.SameAs(annotations[0]));
            Assert.That(set.First.Next,           Is.Not.Null);
            Assert.That(set.First.Next.Item,      Is.SameAs(annotations[1]));
            Assert.That(set.First.Next.Next,      Is.Not.Null);
            Assert.That(set.First.Next.Next.Item, Is.SameAs(annotations[2]));
            Assert.That(set.First.Next.Next.Next, Is.Null);
        }
    }
}
