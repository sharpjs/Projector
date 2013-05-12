namespace Projector
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Projector.ObjectModel;
    using Projector.Specs;

    internal class StandardTraitResolution : ITraitResolution, ITypeScopeAggregator
    {
        private readonly ProjectionType projectionType;
        private readonly Type           underlyingType;

        private List<TypeScope> scopes;

        internal StandardTraitResolution(ProjectionType projectionType, Type underlyingType)
        {
            this.projectionType = projectionType;
            this.underlyingType = underlyingType;
        }

        internal void Add(TraitSpec spec)
        {
            spec.CollectRelevantScopes(projectionType, underlyingType, this);
        }

        void ITypeScopeAggregator.Add(TypeScope scope)
        {
            if (scope == null)
                throw Error.ArgumentNull("scope");

            (scopes ?? (scopes = new List<TypeScope>()))
                .Add(scope);
        }

        public void ResolveTypeTraits(ITraitAggregator aggregator)
        {
            var scopes = this.scopes;
            if (scopes != null)
                foreach (var scope in scopes)
                    scope.Collect(aggregator);

            CollectAttributes(underlyingType, aggregator);
        }

        public void ResolvePropertyTraits(
            ProjectionProperty projectionProperty,
            PropertyInfo       underlyingProperty,
            ITraitAggregator   aggregator)
        {
            var scopes = this.scopes;
            if (scopes != null)
                foreach (var scope in scopes)
                    scope.Collect(projectionProperty, aggregator);

            CollectAttributes(underlyingProperty, aggregator);
        }

        private static void CollectAttributes(MemberInfo source, ITraitAggregator aggregator)
        {
            foreach (var trait in source.GetCustomAttributes(false))
                aggregator.Collect(trait);
        }
    }
}
