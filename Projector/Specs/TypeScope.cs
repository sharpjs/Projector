namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using Projector.ObjectModel;

    internal class TypeScope : ITypeCut, ITypeScope
    {
        public ITypeCut OfKind(TypeKind kind)
        {
            throw new NotImplementedException();
        }

        public ITypeCut OfKind(params TypeKind[] kinds)
        {
            throw new NotImplementedException();
        }

        public ITypeCut Matching(Func<Type, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Spec(Action<ITypeScope> spec)
        {
            throw new NotImplementedException();
        }

        public IPropertyCut Properties
        {
            get { throw new NotImplementedException(); }
        }

        public ITraitScope Property(string name)
        {
            throw new NotImplementedException();
        }

        public ITraitScope Property(params string[] names)
        {
            throw new NotImplementedException();
        }

        public ITraitScope Apply(object trait)
        {
            throw new NotImplementedException();
        }

        public ITraitScope Apply(Func<ITraitContext, object> factory)
        {
            throw new NotImplementedException();
        }
    }

    internal class TypeScope<T> : TypeScope, ITypeScope<T>
    {
        public ITraitScope Property(Expression<Func<T, object>> property)
        {
            throw new NotImplementedException();
        }

        public ITraitScope Property(params Expression<Func<T, object>>[] properties)
        {
            throw new NotImplementedException();
        }
    }
}
