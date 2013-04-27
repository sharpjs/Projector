namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;

    internal sealed class ProjectionTypeTraitAggregator : TraitAggregator<ProjectionStructureType, Type>
    {
        public ProjectionTypeTraitAggregator(ProjectionStructureType type)
            : base(type) { }

        protected override object[] GetDeclaredTraits(ProjectionStructureType projectionType)
        {
            return projectionType.UnderlyingType.GetCustomAttributes(false);
        }

        protected override object[] GetInheritableTraits(ProjectionStructureType projectionType)
        {
            return projectionType.InheritableTraits;
        }

        protected override IEnumerable<ProjectionStructureType> GetInheritanceSources(ProjectionStructureType projectionType)
        {
            // TODO: Don't require structure types here
            return System.Linq.Enumerable.Cast<ProjectionStructureType>(
            projectionType.BaseTypes
            );
        }

        protected override Type GetSourceKey(ProjectionStructureType projectionType)
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
