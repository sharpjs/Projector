namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public static class ProjectionObjectTests
    {
        public abstract class WithProjectionObject
        {
            protected ProjectionObject Target { get; set; }

            internal static readonly object
                KeyA       = new object(),
                KeyB       = new object(),
                MissingKey = new object();

            internal static readonly FakeAssociatedObject
                ObjectA = new FakeAssociatedObject(),
                ObjectB = new FakeAssociatedObject();

            [SetUp]
            public virtual void SetUp()
            {
                Target = new FakeProjectionObject();
            }
        }

        [TestFixture]
        public class Always : WithProjectionObject
        {
            [Test]
            public void HasAssociatedObject_NullKey()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    Target.HasAssociatedObject<FakeAssociatedObject>(null);
                });
            }

            [Test]
            public void GetAssociatedObject_NullKey()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    Target.GetAssociatedObject<FakeAssociatedObject>(null);
                });
            }

            [Test]
            public void TryGetAssociatedObject_NullKey()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    FakeAssociatedObject obj;
                    Target.TryGetAssociatedObject<FakeAssociatedObject>(null, out obj);
                });
            }

            [Test]
            public void SetAssociatedObject_NullKey()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    Target.SetAssociatedObject<FakeAssociatedObject>(null, new FakeAssociatedObject());
                });
            }

            [Test]
            public void SetAssociatedObject_Set()
            {
                var result0 = Target.SetAssociatedObject<FakeAssociatedObject>(KeyA, ObjectA);
                var result1 = Target.SetAssociatedObject<FakeAssociatedObject>(KeyB, ObjectB);

                Assert.That(result0, Is.True);
                Assert.That(result1, Is.True);
            }
        }

        [TestFixture]
        public class Empty : WithProjectionObject
        {
            [Test]
            public void AssociatedObjects()
            {
                Assert.That(Target.AssociatedObjects, Is.Empty);
            }

            [Test]
            public void HasAssociatedObject()
            {
                var result = Target.HasAssociatedObject<FakeAssociatedObject>(MissingKey);

                Assert.That(result, Is.False);
            }

            [Test]
            public void GetAssociatedObject()
            {
                Assert.Throws<KeyNotFoundException>(() =>
                {
                    Target.GetAssociatedObject<FakeAssociatedObject>(MissingKey);
                });
            }

            [Test]
            public void TryGetAssociatedObject()
            {
                FakeAssociatedObject obj;
                var result = Target.TryGetAssociatedObject<FakeAssociatedObject>(MissingKey, out obj);

                Assert.That(result, Is.False);
                Assert.That(obj,    Is.Null);
            }

            [Test]
            public void SetAssociatedObject_Clear()
            {
                var result0 = Target.SetAssociatedObject<FakeAssociatedObject>(KeyA, null);
                var result1 = Target.SetAssociatedObject<FakeAssociatedObject>(KeyB, null);

                Assert.That(result0, Is.False);
                Assert.That(result1, Is.False);
            }
        }

        [TestFixture]
        public class AfterSetAssociatedObjects : WithProjectionObject
        {
            public override void SetUp()
            {
                base.SetUp();
                Target.SetAssociatedObject<FakeAssociatedObject>(KeyA, ObjectA);
                Target.SetAssociatedObject<FakeAssociatedObject>(KeyB, ObjectB);
            }

            [Test]
            public void AssociatedObjects()
            {
                Assert.That(Target.AssociatedObjects, Is.EquivalentTo(new[] { ObjectA, ObjectB }));
            }

            [Test]
            public void HasAssociatedObject_Found()
            {
                var result = Target.HasAssociatedObject<FakeAssociatedObject>(KeyA);

                Assert.That(result, Is.True);
            }

            [Test]
            public void HasAssociatedObject_NotFound()
            {
                var result = Target.HasAssociatedObject<FakeAssociatedObject>(MissingKey);

                Assert.That(result, Is.False);
            }

            [Test]
            public void HasAssociatedObject_WrongType()
            {
                var result = Target.HasAssociatedObject<WrongType>(KeyA);

                Assert.That(result, Is.False);
            }

            [Test]
            public void GetAssociatedObject_Found()
            {
                var obj = Target.GetAssociatedObject<FakeAssociatedObject>(KeyA);

                Assert.That(obj, Is.SameAs(ObjectA));
            }

            [Test]
            public void GetAssociatedObject_NotFound()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => Target.GetAssociatedObject<FakeAssociatedObject>(MissingKey)
                );
            }

            [Test]
            public void GetAssociatedObject_WrongType()
            {
                Assert.Throws<KeyNotFoundException>
                (
                    () => Target.GetAssociatedObject<WrongType>(KeyA)
                );
            }

            [Test]
            public void TryGetAssociatedObject_Found()
            {
                FakeAssociatedObject obj;
                var result = Target.TryGetAssociatedObject<FakeAssociatedObject>(KeyA, out obj);

                Assert.That(result, Is.True);
                Assert.That(obj,    Is.SameAs(ObjectA));
            }

            [Test]
            public void TryGetAssociatedObject_NotFound()
            {
                FakeAssociatedObject obj;
                var result = Target.TryGetAssociatedObject<FakeAssociatedObject>(MissingKey, out obj);

                Assert.That(result, Is.False);
                Assert.That(obj,    Is.Null);
            }

            [Test]
            public void TryGetAssociatedObject_WrongType()
            {
                WrongType obj;
                var result = Target.TryGetAssociatedObject<WrongType>(KeyA, out obj);

                Assert.That(result, Is.False);
                Assert.That(obj,    Is.Null);
            }

            [Test]
            public void SetAssociatedObject_Clear()
            {
                var result0 = Target.SetAssociatedObject<FakeAssociatedObject>(KeyA, null);
                var result1 = Target.SetAssociatedObject<FakeAssociatedObject>(KeyB, null);

                Assert.That(result0, Is.False);
                Assert.That(result1, Is.False);
            }
        }

        [TestFixture]
        public class AfterClearAssociatedObjects : WithProjectionObject
        {
            public override void SetUp()
            {
                base.SetUp();
                Target.SetAssociatedObject<FakeAssociatedObject>(KeyA, ObjectA);
                Target.SetAssociatedObject<FakeAssociatedObject>(KeyB, ObjectB);
                Target.SetAssociatedObject<FakeAssociatedObject>(KeyA, null);
                Target.SetAssociatedObject<FakeAssociatedObject>(KeyB, null);
            }

            [Test]
            public void AssociatedObjects()
            {
                Assert.That(Target.AssociatedObjects, Is.Empty);
            }

            [Test]
            public void HasAssociatedObject()
            {
                var result = Target.HasAssociatedObject<FakeAssociatedObject>(KeyA);

                Assert.That(result, Is.False);
            }

            [Test]
            public void GetAssociatedObject()
            {
                Assert.Throws<KeyNotFoundException>(() =>
                {
                    Target.GetAssociatedObject<FakeAssociatedObject>(KeyA);
                });
            }

            [Test]
            public void TryGetAssociatedObject()
            {
                FakeAssociatedObject obj;
                var result = Target.TryGetAssociatedObject<FakeAssociatedObject>(KeyA, out obj);

                Assert.That(result, Is.False);
                Assert.That(obj,    Is.Null);
            }
        }

        internal sealed class FakeProjectionObject : ProjectionObject
        {
            // ...
        }

        internal sealed class FakeAssociatedObject { }

        private sealed class WrongType { }
    }
}
