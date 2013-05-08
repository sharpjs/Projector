namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Projector.ObjectModel;

    internal class PropertyScope : TraitScope, IPropertyCut, IPropertyScope
    {
        private TypeKind[] kinds;
        private string[] names;


        public IPropertyCut OfKind(TypeKind kind)
        {
            throw new NotImplementedException();
        }

        public IPropertyCut OfKind(params TypeKind[] kinds)
        {
            if (kinds == null)
                throw Error.ArgumentNull("kinds");
            if (this.kinds != null)
                throw Error.TodoError();
            this.kinds = kinds;
            return this;
        }

        public IPropertyCut Named(string name)
        {
            throw new NotImplementedException();
        }

        public IPropertyCut Named(params string[] names)
        {
            throw new NotImplementedException();
        }

        public IPropertyCut Matching(Func<PropertyInfo, bool> predicate)
        {
            throw new NotImplementedException();
        }

        internal void Collect(ProjectionProperty property, ITraitAggregator aggregator)
        {
            var shouldCollect
                =  (names == null || names.Contains(property.Name))
                ;
            if (shouldCollect)
                base.Collect(aggregator);
        }
    }
}
