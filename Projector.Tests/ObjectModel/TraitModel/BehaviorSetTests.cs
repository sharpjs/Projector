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

        [Test]
        public void Override_Initial()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[0]);

            var set = new BehaviorSet(parent);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 0);
        }

        [Test]
        public void Override_AddFirst()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { }
            };

            var parent = new BehaviorSet();
            var set = new BehaviorSet(parent);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 1);
        }

        [Test]
        public void Override_AddSameInstance()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[0]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 0);
        }

        [Test]
        public void Override_AddSameInstance_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { AllowMultiple = true }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[0]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 0);
        }

        [Test]
        public void Override_AddSameType_AtHigherPriority()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2 },
                new BehaviorA { Priority = 1 }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[1]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors.Take(1)));
            AssertOwnedBehaviorCount(parent, set, 1);
        }

        [Test]
        public void Override_AddSameType_AtHigherPriority_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2, AllowMultiple = true },
                new BehaviorA { Priority = 1, AllowMultiple = true }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[1]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 1);
        }

        [Test]
        public void Override_AddSameType_AtLowerPriority()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2 },
                new BehaviorA { Priority = 1 }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[0]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[1]);

            Assert.That(set, Is.EqualTo(behaviors.Skip(1)));
            AssertOwnedBehaviorCount(parent, set, 1);
        }

        [Test]
        public void Override_AddSameType_AtLowerPriority_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2, AllowMultiple = true },
                new BehaviorA { Priority = 1, AllowMultiple = true }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[0]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[1]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 2);
        }

        [Test]
        public void Override_AddAnotherType_AtHigherPriority()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorB { Priority = 2 },
                new BehaviorA { Priority = 1 }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[1]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 1);
        }

        [Test]
        public void Override_AddAnotherType_AtHigherPriority_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorB { Priority = 2, AllowMultiple = true },
                new BehaviorA { Priority = 1, AllowMultiple = true }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[1]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[0]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 1);
        }

        [Test]
        public void Override_AddAnotherType_AtLowerPriority()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2 },
                new BehaviorB { Priority = 1 }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[0]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[1]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 2);
        }

        [Test]
        public void Override_AddAnotherType_AtLowerPriority_AllowMultiple()
        {
            var behaviors = new IProjectionBehavior[]
            {
                new BehaviorA { Priority = 2, AllowMultiple = true },
                new BehaviorB { Priority = 1, AllowMultiple = true }
            };

            var parent = new BehaviorSet();
            parent.Apply(behaviors[0]);

            var set = new BehaviorSet(parent);
            set.Apply(behaviors[1]);

            Assert.That(set, Is.EqualTo(behaviors));
            AssertOwnedBehaviorCount(parent, set, 2);
        }

        private static void AssertOwnedBehaviorCount(BehaviorSet parent, BehaviorSet set, int expected)
        {
            var behavior = set.First;

            for (var i = 0; i < expected; i++)
            {
                Assert.That(behavior, Is.Not.Null, "set[{0}]", i);
                AssertContainsBehavior(parent, behavior, false, i);
                behavior = behavior.Next;
            }

            if (behavior != null)
                AssertContainsBehavior(parent, behavior, true, expected);
        }

        private static void AssertContainsBehavior(BehaviorSet parent, Cell<IProjectionBehavior> behavior, bool expected, int index)
        {
            Assert.That
            (
                Contains(parent.First, behavior),
                Is.EqualTo(expected),
                "parent contains set[{0}]",
                index
            );
        }

        private static bool Contains<T>(Cell<T> list, Cell<T> cell)
        {
            for (; list != null; list = list.Next)
                if (list == cell)
                    return true;

            return false;
        }
    }
}
