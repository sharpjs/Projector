namespace Projector.ObjectModel
{
    internal sealed class IdentityConverter<T> : IConverter<T, T>
    {
        public T ToOuter(T inner)
        {
            return inner;
        }

        public T ToInner(T outer)
        {
            return outer;
        }
    }
}
