namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Specifies the general kind of a type.
    /// </summary>
    public enum TypeKind
    {
        /// <summary>
        /// An opaque type.
        /// Instances are stored and retrieved whole.
        /// This kind includes all concrete types.
        /// </summary>
        Opaque = 0,

        /// <summary>
        /// A complex type with structure semantics.
        /// This kind includes all non-collection interface types.
        /// </summary>
        Structure,

        /// <summary>
        /// A collection type with array semantics.
        /// Includes all one-dimensional array types.
        /// </summary>
        Array,

        /// <summary>
        /// A collection type with list semantics.
        /// Includes all types declared as generic
        ///   <c>IBindingList</c>, <c>IList</c>, <c>ICollection</c>, or <c>IEnumerable</c>.
        /// </summary>
        List,

        /// <summary>
        /// A collection type with set semantics.
        /// Includes all types declared as generic <c>ISet</c>.
        /// </summary>
        Set,

        /// <summary>
        /// A collection type with dictionary semantics.
        /// Includes all types declared as generic <c>IDictionary</c>.
        /// </summary>
        Dictionary
    }

    public static class TypeKindExtensions
    {
        public static bool IsCollection(this TypeKind kind)
        {
            return kind > TypeKind.Structure;
        }

        internal static TypeKind Classify(this Type type)
        {
            if (type.IsArray)
                return TypeKind.Array;

            if (type.IsGenericType)
            {
                TypeKind kind;
                var genericType = type.GetGenericTypeDefinition();

                if (SupportedCollectionTypes.TryGetValue(genericType, out kind))
                    return kind;

                if (UnsupportedCollectionTypes.Contains(genericType))
                    throw Error.UnsupportedCollectionType(type);
            }

            return type.IsInterface
                ? TypeKind.Structure
                : TypeKind.Opaque;
        }

        private static readonly Dictionary<Type, TypeKind>
            SupportedCollectionTypes = new Dictionary<Type, TypeKind>
        {
            { typeof(IEnumerable<>),  TypeKind.List       },
            { typeof(ICollection<>),  TypeKind.List       },
            { typeof(IList<>),        TypeKind.List       },
            { typeof(IBindingList<>), TypeKind.List       },
            { typeof(ISet<>),         TypeKind.Set        },
            { typeof(IBindingSet<>),  TypeKind.Set        },
            { typeof(IDictionary<,>), TypeKind.Dictionary }
        };

        private static readonly HashSet<Type>
            UnsupportedCollectionTypes = new HashSet<Type>
        {
            // System.Collections.Generic
            typeof(List<>),
            typeof(Stack<>),
            typeof(Queue<>),
            typeof(LinkedList<>),
            typeof(SortedList<,>),
            typeof(BindingList<>),
            typeof(HashSet<>),
            typeof(SortedSet<>),
            typeof(Dictionary<,>),
            typeof(SortedDictionary<,>),
            // System.Collections.Concurrent
            typeof(ConcurrentBag<>),
            typeof(ConcurrentStack<>),
            typeof(ConcurrentQueue<>),
            typeof(ConcurrentDictionary<,>),
            // System.Linq
            typeof(Lookup<,>)
        };
    }
}
