namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class PropertyScope : IPropertyCut, IPropertyScope
    {
        public IPropertyCut OfKind(ObjectModel.TypeKind kind)
        {
            throw new NotImplementedException();
        }

        public IPropertyCut OfKind(params ObjectModel.TypeKind[] kinds)
        {
            throw new NotImplementedException();
        }

        public IPropertyCut Named(string name)
        {
            throw new NotImplementedException();
        }

        public IPropertyCut Named(params string[] names)
        {
            throw new NotImplementedException();
        }

        public IPropertyCut Matching(Func<System.Reflection.PropertyInfo, bool> predicate)
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
}
