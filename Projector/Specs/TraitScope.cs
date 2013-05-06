namespace Projector.Specs
{
    using System;

    internal class TraitScope : ITraitScope
    {
        public ITraitScope Apply(object trait)
        {
            throw new NotImplementedException();
        }

        public ITraitScope Apply(Func<ITraitContext, object> factory)
        {
            throw new NotImplementedException();
        }
    }
}
