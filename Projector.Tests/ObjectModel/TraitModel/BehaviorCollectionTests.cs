namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class BehaviorCollectionTests
    {
        [Test]
        public void AsIList_IsReadOnly()
        {
            var collection = new BehaviorCollection() as ICollection<IProjectionBehavior>;

            Assert.That(collection.IsReadOnly, Is.True);
        }

        [Test]
        public void AsIList_Add()
        {
            var collection = new BehaviorCollection() as ICollection<IProjectionBehavior>;

            Assert.Throws<NotSupportedException>(() => collection.Add(null));
        }

        [Test]
        public void AsIList_Remove()
        {
            var collection = new BehaviorCollection() as ICollection<IProjectionBehavior>;

            Assert.Throws<NotSupportedException>(() => collection.Remove(null));
        }

        [Test]
        public void AsIList_Clear()
        {
            var collection = new BehaviorCollection() as ICollection<IProjectionBehavior>;

            Assert.Throws<NotSupportedException>(() => collection.Clear());
        }

        [Test]
        public void Contains_False()
        {
            var a = new BehaviorA();
            var collection = Collection();

            Assert.That(collection.Contains(a), Is.False);
        }

        [Test]
        public void Contains_True()
        {
            var a = new BehaviorA();
            var b = new BehaviorB();
            var collection = Collection(a, b);

            Assert.That(collection.Contains(a), Is.True);
            Assert.That(collection.Contains(b), Is.True);
        }

        [Test]
        public void Initial()
        {
            var set = Collection();

            Assert.That(set.EnumerateGeneric(),    Is.Empty);
            Assert.That(set.EnumerateNongeneric(), Is.Empty);
            Assert.That(set.First,                 Is.Null);
        }

        [Test]
        public void Apply_One()
        {
            var a = new BehaviorA { };

            var set = Collection(a);

            Assert.That(set.EnumerateGeneric(),    HasBehaviors(a));
            Assert.That(set.EnumerateNongeneric(), HasBehaviors(a));
            Assert.That(set.First,                 Is.Not.Null);
            Assert.That(set.First.Item,            Is.SameAs(a));
            Assert.That(set.First.Next,            Is.Null);
        }

        [Test]
        public void Apply_SameInstance()
        {
            var a = new BehaviorA { };

            var set = Collection(a, a);

            Assert.That(set, HasBehaviors(a));
        }

        [Test]
        public void Apply_SameInstance_AllowMultiple()
        {
            var a = new BehaviorA { AllowMultiple = true };

            var set = Collection(a, a);

            Assert.That(set, HasBehaviors(a));
        }

        [Test]
        public void Apply_AnotherType_ThenSameInstance()
        {
            var a = new BehaviorA { };
            var b = new BehaviorB { };

            var set = Collection(a, b, a);

            Assert.That(set, HasBehaviors(a, b));
        }

        [Test]
        public void Apply_AnotherType_ThenSameInstance_AllowMultiple()
        {
            var a = new BehaviorA { AllowMultiple = true };
            var b = new BehaviorB { AllowMultiple = true };

            var set = Collection(a, b, a);

            Assert.That(set, HasBehaviors(a, b));
        }

        [Test]
        public void Apply_SameType_AtHigherPriority()
        {
            var a1 = new BehaviorA { Priority = 1 };
            var b1 = new BehaviorB { Priority = 1 };
            var a2 = new BehaviorA { Priority = 2 };

            var set = Collection(a1, b1, a2);

            Assert.That(set, HasBehaviors(a2, b1));
        }

        [Test]
        public void Apply_SameType_AtHigherPriority_AllowMultiple()
        {
            var a1 = new BehaviorA { Priority = 1, AllowMultiple = true };
            var b1 = new BehaviorB { Priority = 1, AllowMultiple = true };
            var a2 = new BehaviorA { Priority = 2, AllowMultiple = true };

            var set = Collection(a1, b1, a2);

            Assert.That(set, HasBehaviors(a2, b1, a1));
        }

        [Test]
        public void Apply_SameType_AtMiddlePriority()
        {
            var a2 = new BehaviorA { Priority = 2 };
            var b2 = new BehaviorB { Priority = 2 };
            var c0 = new BehaviorC { Priority = 0 };
            var a1 = new BehaviorA { Priority = 1 };

            var set = Collection(a2, b2, c0, a1);

            Assert.That(set, HasBehaviors(b2, a1, c0));
        }

        [Test]
        public void Apply_SameType_AtMiddlePriority_AllowMultiple()
        {
            var a2 = new BehaviorA { Priority = 2, AllowMultiple = true };
            var b2 = new BehaviorB { Priority = 2, AllowMultiple = true };
            var c0 = new BehaviorC { Priority = 0, AllowMultiple = true };
            var a1 = new BehaviorA { Priority = 1, AllowMultiple = true };

            var set = Collection(a2, b2, c0, a1);

            Assert.That(set, HasBehaviors(b2, a2, a1, c0));
        }

        [Test]
        public void Apply_SameType_AtLowerPriority()
        {
            var a2 = new BehaviorA { Priority = 2 };
            var b2 = new BehaviorB { Priority = 2 };
            var a1 = new BehaviorA { Priority = 1 };

            var set = Collection(a2, b2, a1);

            Assert.That(set, HasBehaviors(b2, a1));
        }

        [Test]
        public void Apply_SameType_AtLowerPriority_AllowMultiple()
        {
            var a2 = new BehaviorA { Priority = 2, AllowMultiple = true };
            var b2 = new BehaviorB { Priority = 2, AllowMultiple = true };
            var a1 = new BehaviorA { Priority = 1, AllowMultiple = true };

            var set = Collection(a2, b2, a1);

            Assert.That(set, HasBehaviors(b2, a2, a1));
        }

        [Test]
        public void Apply_AnotherType_AtHigherPriority()
        {
            var a1 = new BehaviorA { Priority = 1 };
            var b1 = new BehaviorB { Priority = 1 };
            var c2 = new BehaviorC { Priority = 2 };

            var set = Collection(a1, b1, c2);

            Assert.That(set, HasBehaviors(c2, b1, a1));
        }

        [Test]
        public void Apply_AnotherType_AtHigherPriority_AllowMultiple()
        {
            var a1 = new BehaviorA { Priority = 1, AllowMultiple = true };
            var b1 = new BehaviorB { Priority = 1, AllowMultiple = true };
            var c2 = new BehaviorC { Priority = 2, AllowMultiple = true };

            var set = Collection(a1, b1, c2);

            Assert.That(set, HasBehaviors(c2, b1, a1));
        }

        [Test]
        public void Apply_AnotherType_AtMiddlePriority()
        {
            var a0 = new BehaviorA { Priority = 0 };
            var b2 = new BehaviorB { Priority = 2 };
            var c1 = new BehaviorC { Priority = 1 };

            var set = Collection(a0, b2, c1);

            Assert.That(set, HasBehaviors(b2, c1, a0));
        }

        [Test]
        public void Apply_AnotherType_AtMiddlePriority_AllowMultiple()
        {
            var a0 = new BehaviorA { Priority = 0, AllowMultiple = true };
            var b2 = new BehaviorB { Priority = 2, AllowMultiple = true };
            var c1 = new BehaviorC { Priority = 1, AllowMultiple = true };

            var set = Collection(a0, b2, c1);

            Assert.That(set, HasBehaviors(b2, c1, a0));
        }

        [Test]
        public void Apply_AnotherType_AtLowerPriority()
        {
            var a2 = new BehaviorA { Priority = 2 };
            var b2 = new BehaviorB { Priority = 2 };
            var c1 = new BehaviorC { Priority = 1 };

            var set = Collection(a2, b2, c1);

            Assert.That(set, HasBehaviors(b2, a2, c1));
        }

        [Test]
        public void Apply_AnotherType_AtLowerPriority_AllowMultiple()
        {
            var a2 = new BehaviorA { Priority = 2, AllowMultiple = true };
            var b2 = new BehaviorB { Priority = 2, AllowMultiple = true };
            var c1 = new BehaviorC { Priority = 1, AllowMultiple = true };

            var set = Collection(a2, b2, c1);

            Assert.That(set, HasBehaviors(b2, a2, c1));
        }

        private static BehaviorCollection Collection(params IProjectionBehavior[] behaviors)
        {
            var set = new BehaviorCollection();

            foreach (var behavior in behaviors)
                set.AddInternal(behavior);

            return set;
        }

        private static void Assert_HasBehaviors(BehaviorCollection collection, params IProjectionBehavior[] behaviors)
        {
            Assert.That(collection.Count,                 Is.EqualTo(behaviors.Length));
            Assert.That(collection.EnumerateGeneric(),    Is.EqualTo(behaviors));
            Assert.That(collection.EnumerateNongeneric(), Is.EqualTo(behaviors));
        }

        private static Constraint HasBehaviors(params IProjectionBehavior[] behaviors)
        {
            return Is.EqualTo(behaviors);
        }
    }
}
