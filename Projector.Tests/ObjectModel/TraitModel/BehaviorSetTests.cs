namespace Projector.Tests.ObjectModel
{
    using System.Linq;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class BehaviorSetTests
    {
        [Test]
        public void Initial()
        {
            var set = new BehaviorSet();

            Assert.That(set,       Is.Empty);
            Assert.That(set.First, Is.Null);
        }

        [Test]
        public void AddFirst()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[0]);

            Assert.That(set,            Is.EqualTo(behaviors));
            Assert.That(set.First,      Is.Not.Null);
            Assert.That(set.First.Item, Is.SameAs(behaviors[0]));
        }

        [Test]
        public void AddSameInstance()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[0]);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
        }

        [Test]
        public void AddSameInstance_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { AllowMultiple = true }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[0]);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
        }

        [Test]
        public void AddSameType_AtHigherPriority()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2 },
                new BehaviorA { Priority = 1 }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[1]);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors.Take(1)));
        }

        [Test]
        public void AddSameType_AtHigherPriority_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2, AllowMultiple = true },
                new BehaviorA { Priority = 1, AllowMultiple = true }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[1]);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
        }

        [Test]
        public void AddSameType_AtLowerPriority()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2 },
                new BehaviorA { Priority = 1 }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[0]);
            set.Apply(behaviors[1]);

            Assert.That(set, Is.EqualTo(behaviors.Skip(1)));
        }

        [Test]
        public void AddSameType_AtLowerPriority_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2, AllowMultiple = true },
                new BehaviorA { Priority = 1, AllowMultiple = true }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[0]);
            set.Apply(behaviors[1]);

            Assert.That(set, Is.EqualTo(behaviors));
        }

        [Test]
        public void AddAnotherType_AtHigherPriority()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorB { Priority = 2 },
                new BehaviorA { Priority = 1 }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[1]);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
        }

        [Test]
        public void AddAnotherType_AtHigherPriority_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorB { Priority = 2, AllowMultiple = true },
                new BehaviorA { Priority = 1, AllowMultiple = true }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[1]);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
        }

        [Test]
        public void AddAnotherType_AtLowerPriority()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2 },
                new BehaviorB { Priority = 1 }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[0]);
            set.Apply(behaviors[1]);

            Assert.That(set, Is.EqualTo(behaviors));
        }

        [Test]
        public void AddAnotherType_AtLowerPriority_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2, AllowMultiple = true },
                new BehaviorB { Priority = 1, AllowMultiple = true }
            };

            var set = new BehaviorSet();
            set.Apply(behaviors[0]);
            set.Apply(behaviors[1]);

            Assert.That(set, Is.EqualTo(behaviors));
        }
    }
}
