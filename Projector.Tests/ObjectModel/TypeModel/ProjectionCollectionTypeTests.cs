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
        public class ArrayType : SupportedType<Item[]>
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
        public class IEnumerableType : SupportedType<IEnumerable<Item>>
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
        public class ICollectionType : SupportedType<ICollection<Item>>
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
        public class IListType : SupportedType<IList<Item>>
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
        public class IBindingListType : SupportedType<IBindingList<Item>>
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
        public class ISetType : SupportedType<ISet<Item>>
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
        public class IBindingSetType : SupportedType<IBindingSet<Item>>
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
        public class IDictionaryType : SupportedType<IDictionary<Key, Item>>
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
        [TestFixture] public class ListType                 : UnsupportedType<List                <     Item>> { }
        [TestFixture] public class StackType                : UnsupportedType<Stack               <     Item>> { }
        [TestFixture] public class QueueType                : UnsupportedType<Queue               <     Item>> { }
        [TestFixture] public class LinkedListType           : UnsupportedType<LinkedList          <     Item>> { }
        [TestFixture] public class HashSetType              : UnsupportedType<HashSet             <     Item>> { }
        [TestFixture] public class SortedSetType            : UnsupportedType<SortedSet           <     Item>> { }
        [TestFixture] public class DictionaryType           : UnsupportedType<Dictionary          <Key, Item>> { }
        [TestFixture] public class SortedDictionaryType     : UnsupportedType<SortedDictionary    <Key, Item>> { }
        [TestFixture] public class SortedListType           : UnsupportedType<SortedList          <Key, Item>> { }
        // System.Collections.Concurrent
        [TestFixture] public class ConcurrentBagType        : UnsupportedType<ConcurrentBag       <     Item>> { }
        [TestFixture] public class ConcurrentStackType      : UnsupportedType<ConcurrentStack     <     Item>> { }
        [TestFixture] public class ConcurrentQueueType      : UnsupportedType<ConcurrentQueue     <     Item>> { }
        [TestFixture] public class ConcurrentDictionaryType : UnsupportedType<ConcurrentDictionary<Key, Item>> { }
        // System.ComponentModel
        [TestFixture] public class BindingListType          : UnsupportedType<BindingList         <     Item>> { }
        // System.Linq
        [TestFixture] public class LookupType               : UnsupportedType<Lookup              <Key, Item>> { }

        public sealed class Key  { }
        public sealed class Item { }

        public abstract class SupportedType<TCollection> : ProjectionTestsBase
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
            public void CollectionKeyType()
            {
                Assert.That(Type.CollectionKeyType, Is.SameAs(ExpectedKeyType));
            }

            [Test]
            public void CollectionItemType()
            {
                Assert.That(Type.CollectionItemType, Is.SameAs(ExpectedItemType));
            }

            [Test]
            public void IsVirtualizable()
            {
                Assert.That(Type.IsVirtualizable, Is.EqualTo(ExpectedIsVirtualizable));
            }

            [Test]
            public void BaseStructureTypes()
            {
                Assert.That(Type.BaseStructureTypes, Is.Empty);
            }

            [Test]
            public void Properties()
            {
                Assert.That(Type.Properties, Is.Empty);
            }
        }

        public abstract class UnsupportedType<TCollection> : ProjectionTestsBase
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
