namespace Projector.Utility
{
    using System;
    using System.Collections;
    using NUnit.Framework;

    [TestFixture]
    public class CellEnumeratorTests
    {
        // Edge cases only; happy paths are tested in CellTests

        [Test]
        public void Current_BeforeFirstItem()
        {
            using (var e = Cell.Cons("a").GetEnumerator())
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    var _ = e.Current;
                });
            }
        }

        [Test]
        public void Current_AfterLastItem()
        {
            using (var e = Cell.Cons("a").GetEnumerator())
            {
                e.MoveNext();
                e.MoveNext();

                Assert.Throws<InvalidOperationException>(() =>
                {
                    var _ = e.Current;
                });
            }
        }

        [Test]
        public void Reset()
        {
            using (var e = Cell.Cons("a").GetEnumerator())
            {
                e.MoveNext();
                e.MoveNext();

                Assert.Throws<NotSupportedException>(() =>
                {
                    (e as IEnumerator).Reset();
                });
            }
        }
    }
}
