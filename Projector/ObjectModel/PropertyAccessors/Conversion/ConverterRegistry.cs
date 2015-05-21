namespace Projector.ObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class ConverterRegistry
    {
        private static readonly Dictionary<Key, object>
            Converters = new Dictionary<Key, object>(KeyComparer.Instance);

        public static void Register<TOuter, TInner>(IConverter<TOuter, TInner> converter)
        {
            if (converter == null)
                throw Error.ArgumentNull("converter");

            Converters[new Key(typeof(TOuter), typeof(TInner))] = converter;
        }

        public static IConverter<TOuter, TInner> GetConverter<TOuter, TInner>()
        {
            var outer = typeof(TOuter);
            var inner = typeof(TInner);
            object converter;

            if (Converters.TryGetValue(new Key(outer, inner), out converter))
                { } // use custom converter
            else if (outer == inner)
                converter = new IdentityConverter<TInner>();
            else if (outer == typeof(object))
                converter = new AnyToObjectConverter<TInner>();

            throw Error.TodoError(); // don't know a conversion
        }

        private struct Key
        {
            public readonly Type OuterType;
            public readonly Type InnerType;

            public Key(Type outerType, Type innerType)
            {
                OuterType = outerType;
                InnerType = innerType;
            }
        }

        private class KeyComparer : IEqualityComparer<Key>
        {
            public static KeyComparer Instance = new KeyComparer();

            private KeyComparer() { }

            public bool Equals(Key keyA, Key keyB)
            {
                return keyA.OuterType == keyB.OuterType
                    && keyA.InnerType == keyB.InnerType;
            }

            public int GetHashCode(Key key)
            {
                return key.OuterType.GetHashCode() * 17
                    +  key.InnerType.GetHashCode();
            }
        }
    }
}
