namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;

    internal sealed class ProjectionTypeTraitAggregator : TraitAggregator<ProjectionType, Type>
    {
        public ProjectionTypeTraitAggregator(ProjectionType type)
            : base(type) { }

        protected override object[] GetDeclaredTraits(ProjectionType projectionType)
        {
            return projectionType.UnderlyingType.GetCustomAttributes(false);
        }

        protected override object[] GetInheritableTraits(ProjectionType projectionType)
        {
            return projectionType.InheritableTraits;
        }

        protected override IEnumerable<ProjectionType> GetInheritanceSources(ProjectionType projectionType)
        {
            return projectionType.BaseTypes;
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
