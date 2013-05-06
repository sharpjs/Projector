namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    // Trait spec about any types, e.g. DomainSharedTraits
    public abstract class TraitSpec
    {
        // Consumers implement spec in constructor
        protected TraitSpec() { }

        protected ITypeCut Types { get { return null; } }

        protected ITypeScope Type<T>() { return null; }

        protected IPropertyCut Properties { get { return null; } }

        internal void Apply(ObjectModel.ProjectionType target, ObjectModel.TraitApplicator applicator)
        {
            // Hmm, not so sure this is the right way
            throw new NotImplementedException();
        }
    }

    // Trait spec about a specific type, e.g. CustomerTraits
    public abstract class TraitSpec<T>
    {
        // Consumers implement spec in constructor
        protected TraitSpec() { }

        protected void Apply(object trait) { }
        protected void Apply(Func<ITraitContext, object> factory) { }

        protected IPropertyCut Properties { get { return null; } }

        protected ITraitScope Property(Expression<Func<T, object>>[] property) { return null; }

        protected ITraitScope Property(string name) { return null; }
    }
}
