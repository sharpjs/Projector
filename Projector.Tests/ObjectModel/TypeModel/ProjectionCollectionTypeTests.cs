namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using NUnit.Framework;

    public static class ProjectionCollectionTypeTests
    {
        [TestFixture]
        public class ArrayType : SupportedCollectionType<Item[]>
        {
            public ArrayType()
            {
                ExpectedMetatype        = typeof(ProjectionArrayType);
                ExpectedKind            = TypeKind.Array;
                ExpectedKeyType         = TypeOf<int>();
                ExpectedItemType        = TypeOf<Item>();
                ExpectedIsVirtualizable = false;
            }
        }

        [TestFixture]
        public class IEnumerableType : SupportedCollectionType<IEnumerable<Item>>
        {
            public IEnumerableType()
            {
                ExpectedMetatype        = typeof(ProjectionListType);
                ExpectedKind            = TypeKind.List;
                ExpectedKeyType         = TypeOf<int>();
                ExpectedItemType        = TypeOf<Item>();
                ExpectedIsVirtualizable = true;
            }
        }

        [TestFixture]
        public class ICollectionType : SupportedCollectionType<ICollection<Item>>
        {
            public ICollectionType()
            {
                ExpectedMetatype        = typeof(ProjectionListType);
                ExpectedKind            = TypeKind.List;
                ExpectedKeyType         = TypeOf<int>();
                ExpectedItemType        = TypeOf<Item>();
                ExpectedIsVirtualizable = true;
            }
        }

        [TestFixture]
        public class IListType : SupportedCollectionType<IList<Item>>
        {
            public IListType()
            {
                ExpectedMetatype        = typeof(ProjectionListType);
                ExpectedKind            = TypeKind.List;
                ExpectedKeyType         = TypeOf<int>();
                ExpectedItemType        = TypeOf<Item>();
                ExpectedIsVirtualizable = true;
            }
        }

        [TestFixture]
        public class IBindingListType : SupportedCollectionType<IBindingList<Item>>
        {
            public IBindingListType()
            {
                ExpectedMetatype        = typeof(ProjectionListType);
                ExpectedKind            = TypeKind.List;
                ExpectedKeyType         = TypeOf<int>();
                ExpectedItemType        = TypeOf<Item>();
                ExpectedIsVirtualizable = true;
            }
        }

        [TestFixture]
        public class ISetType : SupportedCollectionType<ISet<Item>>
        {
            public ISetType()
            {
                ExpectedMetatype        = typeof(ProjectionSetType);
                ExpectedKind            = TypeKind.Set;
                ExpectedKeyType         = TypeOf<int>();
                ExpectedItemType        = TypeOf<Item>();
                ExpectedIsVirtualizable = true;
            }
        }

        [TestFixture]
        public class IBindingSetType : SupportedCollectionType<IBindingSet<Item>>
        {
            public IBindingSetType()
            {
                ExpectedMetatype        = typeof(ProjectionSetType);
                ExpectedKind            = TypeKind.Set;
                ExpectedKeyType         = TypeOf<int>();
                ExpectedItemType        = TypeOf<Item>();
                ExpectedIsVirtualizable = true;
            }
        }

        [TestFixture]
        public class IDictionaryType : SupportedCollectionType<IDictionary<Key, Item>>
        {
            public IDictionaryType()
            {
                ExpectedMetatype        = typeof(ProjectionDictionaryType);
                ExpectedKind            = TypeKind.Dictionary;
                ExpectedKeyType         = TypeOf<Key>();
                ExpectedItemType        = TypeOf<Item>();
                ExpectedIsVirtualizable = true;
            }
        }

        // System.Collections.Generic
        [TestFixture] public class ListType                 : UnsupportedCollectionType<List                <     Item>> { }
        [TestFixture] public class StackType                : UnsupportedCollectionType<Stack               <     Item>> { }
        [TestFixture] public class QueueType                : UnsupportedCollectionType<Queue               <     Item>> { }
        [TestFixture] public class LinkedListType           : UnsupportedCollectionType<LinkedList          <     Item>> { }
        [TestFixture] public class HashSetType              : UnsupportedCollectionType<HashSet             <     Item>> { }
        [TestFixture] public class SortedSetType            : UnsupportedCollectionType<SortedSet           <     Item>> { }
        [TestFixture] public class DictionaryType           : UnsupportedCollectionType<Dictionary          <Key, Item>> { }
        [TestFixture] public class SortedDictionaryType     : UnsupportedCollectionType<SortedDictionary    <Key, Item>> { }
        [TestFixture] public class SortedListType           : UnsupportedCollectionType<SortedList          <Key, Item>> { }
        // System.Collections.Concurrent
        [TestFixture] public class ConcurrentBagType        : UnsupportedCollectionType<ConcurrentBag       <     Item>> { }
        [TestFixture] public class ConcurrentStackType      : UnsupportedCollectionType<ConcurrentStack     <     Item>> { }
        [TestFixture] public class ConcurrentQueueType      : UnsupportedCollectionType<ConcurrentQueue     <     Item>> { }
        [TestFixture] public class ConcurrentDictionaryType : UnsupportedCollectionType<ConcurrentDictionary<Key, Item>> { }
        // System.ComponentModel
        [TestFixture] public class BindingListType          : UnsupportedCollectionType<BindingList         <     Item>> { }
        // System.Linq
        [TestFixture] public class LookupType               : UnsupportedCollectionType<Lookup              <Key, Item>> { }

        public sealed class Key  { }
        public sealed class Item { }

        public abstract class SupportedCollectionType<TCollection> : ProjectionTestsBase
        {
            private readonly ProjectionType Type = TypeOf<TCollection>();

            protected Type           ExpectedMetatype;
            protected TypeKind       ExpectedKind;
            protected ProjectionType ExpectedKeyType;
            protected ProjectionType ExpectedItemType;
            protected bool           ExpectedIsVirtualizable;

            [Test]
            public void Metatype()
            {
                Assert.That(Type, Is.InstanceOf(ExpectedMetatype));
            }

            [Test]
            public void Kind()
            {
                Assert.That(Type.Kind, Is.EqualTo(ExpectedKind));
            }

            [Test]
            public void KeyType()
            {
                Assert.That(Type.KeyType, Is.SameAs(ExpectedKeyType));
            }

            [Test]
            public void ItemType()
            {
                Assert.That(Type.ItemType, Is.SameAs(ExpectedItemType));
            }

            [Test]
            public void IsVirtualizable()
            {
                Assert.That(Type.IsVirtualizable, Is.EqualTo(ExpectedIsVirtualizable));
            }
        }

        public abstract class UnsupportedCollectionType<TCollection> : ProjectionTestsBase
        {
            [Test]
            public void Construct()
            {
                Assert.Throws<NotSupportedException>
                (
                    () => TypeOf<TCollection>()
                );
            }
        }
    }
}
