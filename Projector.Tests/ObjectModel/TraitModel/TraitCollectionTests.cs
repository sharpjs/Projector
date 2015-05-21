namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class TraitCollectionTests
    {
        [Test]
        public void AsIList_IsReadOnly()
        {
            var collection = Collection() as ICollection<object>;

            Assert.That(collection.IsReadOnly, Is.True);
        }

        [Test]
        public void AsIList_Add()
        {
            var collection = Collection() as ICollection<object>;

            Assert.Throws<NotSupportedException>(() => collection.Add(new object()));
        }

        [Test]
        public void AsIList_Remove()
        {
            var collection = Collection() as ICollection<object>;

            Assert.Throws<NotSupportedException>(() => collection.Remove(new object()));
        }

        [Test]
        public void AsIList_Clear()
        {
            var collection = Collection() as ICollection<object>;

            Assert.Throws<NotSupportedException>(() => collection.Clear());
        }

        [Test]
        public void Contains_False()
        {
            var a = new AnnotationA();
            var collection = Collection();

            Assert.That(collection.Contains(a), Is.False);
        }

        [Test]
        public void Contains_True()
        {
            var a = new AnnotationA();
            var b = new AnnotationB();
            var collection = Collection(a, b);

            Assert.That(collection.Contains(a), Is.True);
            Assert.That(collection.Contains(b), Is.True);
        }

        // TODO: Tests for Find()

        [Test]
        public void Initial()
        {
            var collection = Collection();

            Assert_HasTraits(collection); // Empty
        }

        [Test]
        public void Add_One()
        {
            var a = new object();

            var collection = Collection(a);

            Assert_HasTraits(collection, a);
        }

        [Test]
        public void Add_SameInstance()
        {
            var a = new AnnotationA { };

            var collection = Collection(a, a);

            Assert_HasTraits(collection, a);
        }

        [Test]
        public void Add_SameInstance_AllowMultiple()
        {
            var a = new AnnotationA { AllowMultiple = true };

            var collection = Collection(a, a);

            Assert_HasTraits(collection, a);
        }

        [Test]
        public void Add_AnotherType_ThenSameInstance()
        {
            var a = new AnnotationA { };
            var b = new AnnotationB { };

            var collection = Collection(a, b, a);

            // NOTE: Unlike BehaviorCollection, adding same instance to TraitCollection does NOT reorder items
            Assert_HasTraits(collection, a, b);
        }

        [Test]
        public void Add_AnotherType_ThenSameInstance_AllowMultiple()
        {
            var a = new AnnotationA { AllowMultiple = true };
            var b = new AnnotationB { AllowMultiple = true };

            var collection = Collection(a, b, a);

            // NOTE: Unlike BehaviorCollection, adding same instance to TraitCollection does NOT reorder items
            Assert_HasTraits(collection, a, b);
        }

        [Test]
        public void Add_AnotherType_ThenSameType()
        {
            var a1 = new AnnotationA { };
            var b1 = new AnnotationB { };
            var a2 = new AnnotationA { };

            var collection = Collection(a1, b1, a2);

            Assert_HasTraits(collection, b1, a2);
        }

        [Test]
        public void Add_AnotherType_ThenSameType_AllowMultiple()
        {
            var a1 = new AnnotationA { AllowMultiple = true };
            var b1 = new AnnotationB { AllowMultiple = true };
            var a2 = new AnnotationA { AllowMultiple = true };

            var collection = Collection(a1, b1, a2);

            Assert_HasTraits(collection, a1, b1, a2);
        }

        private static TraitCollection Collection(params object[] traits)
        {
            var collection = new TraitCollection();

            foreach (var trait in traits)
                collection.AddInternal(trait, trait.GetTraitOptions().Inherited);

            collection.Trim();

            return collection;
        }

        private static void Assert_HasTraits(TraitCollection collection, params object[] traits)
        {
            Assert.That(collection.Count,                 Is.EqualTo(traits.Length));
            Assert.That(collection.EnumerateGeneric(),    Is.EqualTo(traits));
            Assert.That(collection.EnumerateNongeneric(), Is.EqualTo(traits));
        }
    }
}
