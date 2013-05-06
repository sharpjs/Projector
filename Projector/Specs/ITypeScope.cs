namespace Projector.Specs
{
    using System;
    using System.Linq.Expressions;

    public interface ITypeScope : ITraitScope
    {
        IPropertyCut Properties { get; }

        ITraitScope Property(       string   name );
        ITraitScope Property(params string[] names);
    }

    public interface ITypeScope<T> : ITraitScope
    {
        ITraitScope Property(       Expression<Func<T, object>>   property  );
        ITraitScope Property(params Expression<Func<T, object>>[] properties);
    }
}
