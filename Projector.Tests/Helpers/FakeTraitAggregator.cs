namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    internal class FakeTraitAggregator : ITraitAggregator
    {
        private readonly List<object> traits;

        public FakeTraitAggregator()
        {
            traits = new List<object>();
        }

        public List<object> Traits
        {
            get { return traits; }
        }

        public void Add(object trait)
        {
            traits.Add(trait);
        }
    }
}
