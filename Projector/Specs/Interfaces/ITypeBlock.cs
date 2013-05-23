namespace Projector.Specs
{
    using System;

    public interface ITypeBlock : ITypeScope
    {
        void Spec(Action<ITypeScope> spec);
    }

    public interface ITypeBlock<T> : ITypeScope<T>
    {
        void Spec(Action<ITypeScope<T>> spec);
    }
}
