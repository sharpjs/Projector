namespace Projector.Tests.ObjectModel
{
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using Projector.ObjectModel;

    [TestFixture]
    public class BehaviorSetTests
    {
        [Test]
        public void Initial()
        {
            var set = BehaviorSet();

            Assert.That(set,       Is.Empty);
            Assert.That(set.First, Is.Null);
        }

        [Test]
        public void Apply_One()
        {
            var a = new BehaviorA { };

            var set = BehaviorSet(a);

            Assert.That(set,            HasBehaviors(a));
            Assert.That(set.First,      Is.Not.Null);
            Assert.That(set.First.Item, Is.SameAs(a));
            Assert.That(set.First.Next, Is.Null);
        }

        [Test]
        public void Apply_SameInstance()
        {
            var a = new BehaviorA { };

            var set = BehaviorSet(a, a);

            Assert.That(set, HasBehaviors(a));
        }

        [Test]
        public void Apply_SameInstance_AllowMultiple()
        {
            var a = new BehaviorA { AllowMultiple = true };

            var set = BehaviorSet(a, a);

            Assert.That(set, HasBehaviors(a));
        }

        [Test]
        public void Apply_AnotherType_ThenSameInstance()
        {
            var a = new BehaviorA { };
            var b = new BehaviorB { };

            var set = BehaviorSet(a, b, a);

            Assert.That(set, HasBehaviors(a, b));
        }

        [Test]
        public void Apply_AnotherType_ThenSameInstance_AllowMultiple()
        {
            var a = new BehaviorA { AllowMultiple = true };
            var b = new BehaviorB { AllowMultiple = true };

            var set = BehaviorSet(a, b, a);

            Assert.That(set, HasBehaviors(a, b));
        }

        [Test]
        public void Apply_SameType_AtHigherPriority()
        {
            var a1 = new BehaviorA { Priority = 1 };
            var b1 = new BehaviorB { Priority = 1 };
            var a2 = new BehaviorA { Priority = 2 };

            var set = BehaviorSet(a1, b1, a2);

            Assert.That(set, HasBehaviors(a2, b1));
        }

        [Test]
        public void Apply_SameType_AtHigherPriority_AllowMultiple()
        {
            var a1 = new BehaviorA { Priority = 1, AllowMultiple = true };
            var b1 = new BehaviorB { Priority = 1, AllowMultiple = true };
            var a2 = new BehaviorA { Priority = 2, AllowMultiple = true };

            var set = BehaviorSet(a1, b1, a2);

            Assert.That(set, HasBehaviors(a2, b1, a1));
        }

        [Test]
        public void Apply_SameType_AtMiddlePriority()
        {
            var a2 = new BehaviorA { Priority = 2 };
            var b2 = new BehaviorB { Priority = 2 };
            var c0 = new BehaviorC { Priority = 0 };
            var a1 = new BehaviorA { Priority = 1 };

            var set = BehaviorSet(a2, b2, c0, a1);

            Assert.That(set, HasBehaviors(b2, a1, c0));
        }

        [Test]
        public void Apply_SameType_AtMiddlePriority_AllowMultiple()
        {
            var a2 = new BehaviorA { Priority = 2, AllowMultiple = true };
            var b2 = new BehaviorB { Priority = 2, AllowMultiple = true };
            var c0 = new BehaviorC { Priority = 0, AllowMultiple = true };
            var a1 = new BehaviorA { Priority = 1, AllowMultiple = true };

            var set = BehaviorSet(a2, b2, c0, a1);

            Assert.That(set, HasBehaviors(b2, a2, a1, c0));
        }

        [Test]
        public void Apply_SameType_AtLowerPriority()
        {
            var a2 = new BehaviorA { Priority = 2 };
            var b2 = new BehaviorB { Priority = 2 };
            var a1 = new BehaviorA { Priority = 1 };

            var set = BehaviorSet(a2, b2, a1);

            Assert.That(set, HasBehaviors(b2, a1));
        }

        [Test]
        public void Apply_SameType_AtLowerPriority_AllowMultiple()
        {
            var a2 = new BehaviorA { Priority = 2, AllowMultiple = true };
            var b2 = new BehaviorB { Priority = 2, AllowMultiple = true };
            var a1 = new BehaviorA { Priority = 1, AllowMultiple = true };

            var set = BehaviorSet(a2, b2, a1);

            Assert.That(set, HasBehaviors(b2, a2, a1));
        }

        [Test]
        public void Apply_AnotherType_AtHigherPriority()
        {
            var a1 = new BehaviorA { Priority = 1 };
            var b1 = new BehaviorB { Priority = 1 };
            var c2 = new BehaviorC { Priority = 2 };

            var set = BehaviorSet(a1, b1, c2);

            Assert.That(set, HasBehaviors(c2, b1, a1));
        }

        [Test]
        public void Apply_AnotherType_AtHigherPriority_AllowMultiple()
        {
            var a1 = new BehaviorA { Priority = 1, AllowMultiple = true };
            var b1 = new BehaviorB { Priority = 1, AllowMultiple = true };
            var c2 = new BehaviorC { Priority = 2, AllowMultiple = true };

            var set = BehaviorSet(a1, b1, c2);

            Assert.That(set, HasBehaviors(c2, b1, a1));
        }

        [Test]
        public void Apply_AnotherType_AtMiddlePriority()
        {
            var a0 = new BehaviorA { Priority = 0 };
            var b2 = new BehaviorB { Priority = 2 };
            var c1 = new BehaviorC { Priority = 1 };

            var set = BehaviorSet(a0, b2, c1);

            Assert.That(set, HasBehaviors(b2, c1, a0));
        }

        [Test]
        public void Apply_AnotherType_AtMiddlePriority_AllowMultiple()
        {
            var a0 = new BehaviorA { Priority = 0, AllowMultiple = true };
            var b2 = new BehaviorB { Priority = 2, AllowMultiple = true };
            var c1 = new BehaviorC { Priority = 1, AllowMultiple = true };

            var set = BehaviorSet(a0, b2, c1);

            Assert.That(set, HasBehaviors(b2, c1, a0));
        }

        [Test]
        public void Apply_AnotherType_AtLowerPriority()
        {
            var a2 = new BehaviorA { Priority = 2 };
            var b2 = new BehaviorB { Priority = 2 };
            var c1 = new BehaviorC { Priority = 1 };

            var set = BehaviorSet(a2, b2, c1);

            Assert.That(set, HasBehaviors(b2, a2, c1));
        }

        [Test]
        public void Apply_AnotherType_AtLowerPriority_AllowMultiple()
        {
            var a2 = new BehaviorA { Priority = 2, AllowMultiple = true };
            var b2 = new BehaviorB { Priority = 2, AllowMultiple = true };
            var c1 = new BehaviorC { Priority = 1, AllowMultiple = true };

            var set = BehaviorSet(a2, b2, c1);

            Assert.That(set, HasBehaviors(b2, a2, c1));
        }

        private static BehaviorSet BehaviorSet(params IProjectionBehavior[] behaviors)
        {
            var set = new BehaviorSet();

            foreach (var behavior in behaviors)
                set.Apply(behavior);

            return set;
        }

        private static Constraint HasBehaviors(params IProjectionBehavior[] behaviors)
        {
            return Is.EqualTo(behaviors);
        }
    }
}
