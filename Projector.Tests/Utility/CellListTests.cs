namespace Projector.Utility
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public class CellListTests
    {
        public abstract class WithCellList
        {
            internal CellList<string> List { get; set; }

            [SetUp]
            public virtual void SetUp()
            {
                List = new CellList<string>();
            }
        }

        [TestFixture]
        public class Always : WithCellList
        {
            [Test]
            public void IsReadOnly()
            {
                Assert.That((List as ICollection<string>).IsReadOnly, Is.False);
            }

            [Test]
            public void CopyTo_NullArray()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    List.CopyTo(null, 0);
                });
            }

            [Test]
            public void CopyTo_InvalidIndexA()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    List.CopyTo(new string[0], -1);
                });
            }

            [Test]
            public void CopyTo_InvalidIndexB()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    List.CopyTo(new string[0], 1);
                });
            }
        }

        [TestFixture]
        public class WhenEmpty : WithCellList
        {
            [Test]
            public void Count()
            {
                Assert.That(List.Count, Is.EqualTo(0));
            }

            [Test]
            public void Head()
            {
                Assert.That(List.Head, Is.Null);
            }

            [Test]
            public void Contains()
            {
                Assert.That(List.Contains("any"), Is.False);
            }

            [Test]
            public void Enqueue()
            {
                var cellA = List.Enqueue("a");
                var cellB = List.Enqueue("b");

                Assert.That(cellA,      Is.Not.Null);
                Assert.That(cellA.Item, Is.EqualTo("a"));
                Assert.That(cellA.Next, Is.SameAs(cellB));
                Assert.That(cellB,      Is.Not.Null);
                Assert.That(cellB.Item, Is.EqualTo("b"));
                Assert.That(cellB.Next, Is.Null);
            }

            [Test]
            public void Push()
            {
                var cellA = List.Push("a");
                var cellB = List.Push("b");

                Assert.That(cellB,      Is.Not.Null);
                Assert.That(cellB.Item, Is.EqualTo("b"));
                Assert.That(cellB.Next, Is.SameAs(cellA));
                Assert.That(cellA,      Is.Not.Null);
                Assert.That(cellA.Item, Is.EqualTo("a"));
                Assert.That(cellA.Next, Is.Null);
            }

            [Test]
            public void Remove()
            {
                Assert.That(List.Remove("any"), Is.False);
            }

            [Test]
            public void TryTake()
            {
                var item  = null as string;
                var taken = List.TryTake(out item);

                Assert.That(item,  Is.Null);
                Assert.That(taken, Is.False);
            }

            [Test]
            public void Enumerate_Generic()
            {
                Assert.That(List.EnumerateGeneric(), Is.Empty);
            }

            [Test]
            public void Enumerate_Nongeneric()
            {
                Assert.That(List.EnumerateNongeneric(), Is.Empty);
            }

            [Test]
            public void CopyTo()
            {
                var items = new string[2];

                List.CopyTo(items, 1);

                Assert.That(items, Is.EqualTo(new string[2]));
            }
        }

        [TestFixture]
        public class AfterEnqueue : WithCellList
        {
            public override void SetUp()
            {
                List = new CellList<string>();
                List.Enqueue("a");
                List.Enqueue("b");
            }

            [Test]
            public void Count()
            {
                Assert.That(List.Count, Is.EqualTo(2));
            }

            [Test]
            public void Head()
            {
                Assert.That(List.Head,           Is.Not.Null);
                Assert.That(List.Head.Item,      Is.EqualTo("a"));
                Assert.That(List.Head.Next,      Is.Not.Null);
                Assert.That(List.Head.Next.Item, Is.EqualTo("b"));
                Assert.That(List.Head.Next.Next, Is.Null);
            }

            [Test]
            public void Contains_True()
            {
                Assert.That(List.Contains("a"), Is.True);
            }

            [Test]
            public void Contains_False()
            {
                Assert.That(List.Contains("other"), Is.False);
            }

            [Test]
            public void Remove_True()
            {
                Assert.That(List.Remove("a"), Is.True);
            }

            [Test]
            public void Remove_False()
            {
                Assert.That(List.Remove("other"), Is.False);
            }

            [Test]
            public void TryTake()
            {
                string item; bool taken;

                taken = List.TryTake(out item);
                Assert.That(taken, Is.True);
                Assert.That(item,  Is.EqualTo("a"));

                taken = List.TryTake(out item);
                Assert.That(taken, Is.True);
                Assert.That(item,  Is.EqualTo("b"));

                taken = List.TryTake(out item);
                Assert.That(taken, Is.False);
                Assert.That(item,  Is.Null);
            }

            [Test]
            public void CopyTo()
            {
                var items = new string[4];

                List.CopyTo(items, 1);

                Assert.That(items, Is.EqualTo(new[] { null, "a", "b", null }));
            }

            [Test]
            public void Enumerate_Generic()
            {
                Assert.That(List.EnumerateGeneric(), Is.EqualTo(new[] { "a", "b" }));
            }

            [Test]
            public void Enumerate_Nongeneric()
            {
                Assert.That(List.EnumerateNongeneric(), Is.EqualTo(new[] { "a", "b" }));
            }
        }

        [TestFixture]
        public class AfterPush : WithCellList
        {
            public override void SetUp()
            {
                List = new CellList<string>();
                List.Push("a");
                List.Push("b");
            }

            [Test]
            public void Count()
            {
                Assert.That(List.Count, Is.EqualTo(2));
            }

            [Test]
            public void Head()
            {
                Assert.That(List.Head,           Is.Not.Null);
                Assert.That(List.Head.Item,      Is.EqualTo("b"));
                Assert.That(List.Head.Next,      Is.Not.Null);
                Assert.That(List.Head.Next.Item, Is.EqualTo("a"));
                Assert.That(List.Head.Next.Next, Is.Null);
            }

            [Test]
            public void Contains_True()
            {
                Assert.That(List.Contains("a"), Is.True);
            }

            [Test]
            public void Contains_False()
            {
                Assert.That(List.Contains("other"), Is.False);
            }

            [Test]
            public void Remove_True()
            {
                Assert.That(List.Remove("a"), Is.True);
            }

            [Test]
            public void Remove_False()
            {
                Assert.That(List.Remove("other"), Is.False);
            }

            [Test]
            public void TryTake()
            {
                string item; bool taken;

                taken = List.TryTake(out item);
                Assert.That(taken, Is.True);
                Assert.That(item,  Is.EqualTo("b"));

                taken = List.TryTake(out item);
                Assert.That(taken, Is.True);
                Assert.That(item,  Is.EqualTo("a"));

                taken = List.TryTake(out item);
                Assert.That(taken, Is.False);
                Assert.That(item,  Is.Null);
            }

            [Test]
            public void CopyTo()
            {
                var items = new string[4];

                List.CopyTo(items, 1);

                Assert.That(items, Is.EqualTo(new[] { null, "b", "a", null }));
            }

            [Test]
            public void Enumerate_Generic()
            {
                Assert.That(List.EnumerateGeneric(), Is.EqualTo(new[] { "b", "a" }));
            }

            [Test]
            public void Enumerate_Nongeneric()
            {
                Assert.That(List.EnumerateNongeneric(), Is.EqualTo(new[] { "b", "a" }));
            }
        }

        [TestFixture]
        public class AfterAdd : AfterEnqueue
        {
            public override void SetUp()
            {
                List = new CellList<string>();
                var collection = List as ICollection<string>;
                collection.Add("a");
                collection.Add("b");
            }
        }

        [TestFixture]
        public class AfterRemoveFirst : WithCellList
        {
            public override void SetUp()
            {
                List = new CellList<string>();
                List.Enqueue("a");
                List.Enqueue("b");
                List.Remove ("a");
            }

            [Test]
            public void Count()
            {
                Assert.That(List.Count, Is.EqualTo(1));
            }

            [Test]
            public void Head()
            {
                Assert.That(List.Head,      Is.Not.Null);
                Assert.That(List.Head.Item, Is.EqualTo("b"));
                Assert.That(List.Head.Next, Is.Null);
            }

            [Test]
            public void Contains_True()
            {
                Assert.That(List.Contains("b"), Is.True);
            }

            [Test]
            public void Contains_False()
            {
                Assert.That(List.Contains("a"), Is.False);
            }

            [Test]
            public void Remove_True()
            {
                Assert.That(List.Remove("b"), Is.True);
            }

            [Test]
            public void Remove_False()
            {
                Assert.That(List.Remove("a"), Is.False);
            }

            [Test]
            public void TryTake()
            {
                string item; bool taken;

                taken = List.TryTake(out item);
                Assert.That(taken, Is.True);
                Assert.That(item,  Is.EqualTo("b"));

                taken = List.TryTake(out item);
                Assert.That(taken, Is.False);
                Assert.That(item,  Is.Null);
            }

            [Test]
            public void CopyTo()
            {
                var items = new string[3];

                List.CopyTo(items, 1);

                Assert.That(items, Is.EqualTo(new[] { null, "b", null }));
            }

            [Test]
            public void Enumerate_Generic()
            {
                Assert.That(List.EnumerateGeneric(), Is.EqualTo(new[] { "b" }));
            }

            [Test]
            public void Enumerate_Nongeneric()
            {
                Assert.That(List.EnumerateNongeneric(), Is.EqualTo(new[] { "b" }));
            }
        }

        [TestFixture]
        public class AfterRemoveSubsequent : WithCellList
        {
            public override void SetUp()
            {
                List = new CellList<string>();
                List.Enqueue("a");
                List.Enqueue("b");
                List.Remove ("b");
            }

            [Test]
            public void Count()
            {
                Assert.That(List.Count, Is.EqualTo(1));
            }

            [Test]
            public void Head()
            {
                Assert.That(List.Head,      Is.Not.Null);
                Assert.That(List.Head.Item, Is.EqualTo("a"));
                Assert.That(List.Head.Next, Is.Null);
            }

            [Test]
            public void Contains_True()
            {
                Assert.That(List.Contains("a"), Is.True);
            }

            [Test]
            public void Contains_False()
            {
                Assert.That(List.Contains("b"), Is.False);
            }

            [Test]
            public void Remove_True()
            {
                Assert.That(List.Remove("a"), Is.True);
            }

            [Test]
            public void Remove_False()
            {
                Assert.That(List.Remove("b"), Is.False);
            }

            [Test]
            public void TryTake()
            {
                string item; bool taken;

                taken = List.TryTake(out item);
                Assert.That(taken, Is.True);
                Assert.That(item,  Is.EqualTo("a"));

                taken = List.TryTake(out item);
                Assert.That(taken, Is.False);
                Assert.That(item,  Is.Null);
            }

            [Test]
            public void CopyTo()
            {
                var items = new string[3];

                List.CopyTo(items, 1);

                Assert.That(items, Is.EqualTo(new[] { null, "a", null }));
            }

            [Test]
            public void Enumerate_Generic()
            {
                Assert.That(List.EnumerateGeneric(), Is.EqualTo(new[] { "a" }));
            }

            [Test]
            public void Enumerate_Nongeneric()
            {
                Assert.That(List.EnumerateNongeneric(), Is.EqualTo(new[] { "a" }));
            }
        }

        [TestFixture]
        public class AfterTakeFirst : AfterRemoveFirst
        {
            public override void SetUp()
            {
                string item;
                List = new CellList<string>();
                List.Enqueue("a");
                List.Enqueue("b");
                List.TryTake(out item);
            }
        }

        [TestFixture]
        public class AfterTakeRemaining : WhenEmpty
        {
            public override void SetUp()
            {
                string item;
                List = new CellList<string>();
                List.Enqueue("a");
                List.Enqueue("b");
                List.TryTake(out item);
                List.TryTake(out item);
            }
        }

        [TestFixture]
        public class AfterClear : WhenEmpty
        {
            public override void SetUp()
            {
                List = new CellList<string>();
                List.Enqueue("a");
                List.Enqueue("b");
                List.Clear();
            }
        }
    }
}
