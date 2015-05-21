namespace Projector.ObjectModel
{
    internal sealed class AnyToObjectConverter<T> : IConverter<T, object>
    {
        public T ToOuter(object inner)
        {
            return (T) inner;
        }

        public object ToInner(T outer)
        {
            return outer;
        }
    }
}
