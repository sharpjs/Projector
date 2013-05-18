namespace Projector.Specs
{
    using System;
    using System.Linq.Expressions;

    public interface ITypeScope : ITraitScope
    {
        IPropertyCut Properties { get; }

        IPropertyScope Property(string name);

        void Spec(Action<ITypeScope> spec);
    }

    public interface ITypeScope<T> : ITypeScope
    {
        IPropertyScope Property(Expression<Func<T, object>> expression);

        new void Spec(Action<ITypeScope<T>> spec);
    }
}
