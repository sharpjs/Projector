namespace Projector.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class CellTests
    {
        [Test]
        public void Cons_Item()
        {
            var cell = Cell.Cons("a");

            Assert.That(cell,      Is.Not.Null);
            Assert.That(cell.Item, Is.EqualTo("a"));
            Assert.That(cell.Next, Is.Null);
        }

        [Test]
        public void Cons_Item_Next()
        {
            var cell = Cell.Cons("a", Cell.Cons("b"));

            Assert.That(cell,           Is.Not.Null);
            Assert.That(cell.Item,      Is.EqualTo("a"));
            Assert.That(cell.Next,      Is.Not.Null);
            Assert.That(cell.Next.Item, Is.EqualTo("b"));
            Assert.That(cell.Next.Next, Is.Null);
        }

        [Test]
        public void Append_First()
        {
            var cell = null as Cell<string>;

            Cell.Append(ref cell, Cell.Cons("a"));

            Assert.That(cell,      Is.Not.Null);
            Assert.That(cell.Item, Is.EqualTo("a"));
            Assert.That(cell.Next, Is.Null);
        }

        [Test]
        public void Append_Subsequent()
        {
            var cell = Cell.Cons("a");

            Cell.Append(ref cell, Cell.Cons("b"));

            Assert.That(cell,           Is.Not.Null);
            Assert.That(cell.Item,      Is.EqualTo("a"));
            Assert.That(cell.Next,      Is.Not.Null);
            Assert.That(cell.Next.Item, Is.EqualTo("b"));
            Assert.That(cell.Next.Next, Is.Null);
        }

        [Test]
        public void Next_Set()
        {
            var cell = Cell.Cons("a");

            cell.Next = Cell.Cons("b");

            Assert.That(cell,           Is.Not.Null);
            Assert.That(cell.Item,      Is.EqualTo("a"));
            Assert.That(cell.Next,      Is.Not.Null);
            Assert.That(cell.Next.Item, Is.EqualTo("b"));
            Assert.That(cell.Next.Next, Is.Null);
        }

        [Test]
        public void Copy()
        {
            var cell = Cell.Cons("a", Cell.Cons("b"));

            var copy = cell.Copy();

            Assert.That(copy,           Is.Not.Null & Is.Not.SameAs(cell));
            Assert.That(copy.Item,      Is.SameAs(cell.Item));
            Assert.That(copy.Next,      Is.SameAs(cell.Next));
        }

        [Test]
        public void ToString_Single()
        {
            var cell = Cell.Cons("a");

            var text = cell.ToString();

            Assert.That(text, Is.EqualTo("[a]"));
        }

        [Test]
        public void ToString_Multiple()
        {
            var cell = Cell.Cons("a", Cell.Cons("b", Cell.Cons("c")));

            var text = cell.ToString();

            Assert.That(text, Is.EqualTo("[a -> b -> c]"));
        }

        [Test]
        public void ToString_Nested()
        {
            var cell = Cell.Cons(Cell.Cons("a"), Cell.Cons(Cell.Cons("b")));

            var text = cell.ToString();

            Assert.That(text, Is.EqualTo("[[a] -> [b]]"));
        }

        [Test]
        public void Enumerate_Generic()
        {
            var cell = Cell.Cons("a", Cell.Cons("b"));

            var items = cell.ToList();

            Assert.That(items, Is.EqualTo(new[] { "a", "b" }));
        }

        [Test]
        public void Enumerate_Nongeneric()
        {
            var cell = Cell.Cons("a", Cell.Cons("b"));

            var items = cell.EnumerateNongeneric();

            Assert.That(items, Is.EqualTo(new[] { "a", "b" }));
        }   
    }
}
