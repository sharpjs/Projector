namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    internal sealed class ProjectionPropertyTraitAggregator : TraitAggregator<ProjectionProperty, MemberKey>
    {
        private readonly PropertyInfo                 underlyingProperty;
        private readonly ProjectionPropertyCollection properties;
        private          List<ProjectionProperty>     overrides;
        private          object[]                     declaredTraits;

        private static System.Collections.ObjectModel.ReadOnlyCollection<ProjectionProperty>
            NoOverrides = Array.AsReadOnly(new ProjectionProperty[0]);

        public ProjectionPropertyTraitAggregator
        (
            ProjectionProperty           property,
            PropertyInfo              underlyingProperty,
            ProjectionPropertyCollection properties
        )
        : base(property)
        {
            this.underlyingProperty = underlyingProperty;
            this.properties         = properties;
        }

        private System.Collections.ObjectModel.ReadOnlyCollection<ProjectionProperty> Overrides
        {
            get { return overrides == null ? NoOverrides : overrides.AsReadOnly(); }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<ProjectionProperty> CollectOverrides()
        {
            declaredTraits = underlyingProperty.GetCustomAttributes(false);
            OverrideAttribute overrideDirective;

            for (var i = 0; i < declaredTraits.Length; i++)
            {
                if (null != (overrideDirective = declaredTraits[i] as OverrideAttribute))
                {
                    AddOverrideDirective(overrideDirective);
                    declaredTraits[i] = null;
                }
            }

            return Overrides;
        }

        private void AddOverrideDirective(OverrideAttribute directive)
        {
            var oldProperty = properties.Override
            (
                directive.MemberName ?? Target.Name,
                directive.SourceType,
                Target
            );

            if (overrides == null)
                overrides = new List<ProjectionProperty>();
            overrides.Add(oldProperty);
        }

        protected override object[] GetDeclaredTraits(ProjectionProperty property)
        {
            return declaredTraits;
        }

        protected override object[] GetInheritableTraits(ProjectionProperty property)
        {
            return property.InheritableTraits;
        }

        protected override IEnumerable<ProjectionProperty> GetInheritanceSources(ProjectionProperty property)
        {
            return overrides == null ? NoOverrides : overrides as IEnumerable<ProjectionProperty>;
        }

        protected override MemberKey GetSourceKey(ProjectionProperty property)
        {
            return new MemberKey(property);
        }

        protected override MemberKey GetSourceKey(InheritFromAttribute directive)
        {
            return new MemberKey(directive.MemberName ?? Target.Name, directive.SourceType);
        }

        protected override IEqualityComparer<MemberKey> SourceKeyComparer
        {
            get { return MemberKeyComparer.Instance; }
        }

        protected override void HandleTraitConflict(Type traitType)
        {
            throw Error.AttributeConflict(Target, traitType);
        }
    }
}
