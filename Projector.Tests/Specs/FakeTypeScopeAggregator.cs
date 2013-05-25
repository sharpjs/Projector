namespace Projector.Specs
{
    using System.Collections.Generic;

    internal class FakeTypeScopeAggregator : ITypeScopeAggregator
    {
        private readonly List<TypeScope> scopes;

        public FakeTypeScopeAggregator()
        {
            scopes = new List<TypeScope>();
        }

        public List<TypeScope> Scopes
        {
            get { return scopes; }
        }

        public void Add(TypeScope scope)
        {
            scopes.Add(scope);
        }
    }
}
