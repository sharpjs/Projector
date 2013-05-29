namespace Projector.ObjectModel
{
    using System;

    internal sealed class ProjectionTypeTraitAggregator
        : TraitAggregator<ProjectionType, Type>, ITraitAggregator
    {
        private ITraitResolution resolution;

        internal ProjectionTypeTraitAggregator(ProjectionType type, ITraitResolution resolution)
            : base(type)
        {
            this.resolution = resolution;
        }

        public override void CollectDeclaredTraits()
        {
            resolution.ProvideTypeTraits(this);
            resolution = null;
        }

        void ITraitAggregator.Add(object trait)
        {
            CollectDeclaredTrait(trait);
        }

        public override void CollectInheritedTraits()
        {
            foreach (var baseType in Target.BaseTypes)
                CollectInheritedTraits(baseType);
        }

        protected override Type GetSourceKey(ProjectionType projectionType)
        {
            return projectionType.UnderlyingType;
        }

        protected override Type GetSourceKey(InheritFromAttribute directive)
        {
            return directive.SourceType;
        }

        protected override void HandleTraitConflict(Type attributeType)
        {
            throw Error.AttributeConflict(Target, attributeType);
        }
    }
}
