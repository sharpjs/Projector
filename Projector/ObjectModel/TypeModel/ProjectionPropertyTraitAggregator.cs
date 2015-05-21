namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal sealed class ProjectionPropertyTraitAggregator
        : TraitAggregator<ProjectionProperty, MemberKey>, ITraitAggregator
    {
        private readonly ProjectionPropertyCollection properties;
        private          List<ProjectionProperty>     overrides;
        private          List<object>                 declaredTraits;

        private BehaviorSet<IProjectionBehavior> initializers;

        private static readonly ReadOnlyCollection<ProjectionProperty>
            NoOverrides = Array.AsReadOnly(new ProjectionProperty[0]);

        internal ProjectionPropertyTraitAggregator(ProjectionProperty property, ProjectionPropertyCollection properties)
            : base(property)
        {
            this.properties = properties;
            initializers = new BehaviorSet<IProjectionBehavior>();
        }

        public ReadOnlyCollection<ProjectionProperty> Overrides
        {
            get { return overrides == null ? NoOverrides : overrides.AsReadOnly(); }
        }

        void ITraitAggregator.Add(object trait)
        {
            OverrideAttribute overrideDirective;

            if (trait == null)
                return; // TODO: Should we be lenient if resolver provides a null trait?
            else if (null != (overrideDirective = trait as OverrideAttribute))
                AddOverrideDirective(overrideDirective);
            else
                QueueDeclaredTrait(trait);
        }

        private void AddOverrideDirective(OverrideAttribute directive)
        {
            var oldProperty = properties.Override
            (
                directive.MemberName ?? Target.Name,
                directive.SourceType,
                Target
            );

            var overrides = this.overrides;
            if (overrides == null)
                overrides = this.overrides = new List<ProjectionProperty>();
            overrides.Add(oldProperty);
        }

        private void QueueDeclaredTrait(object trait)
        {
            var declaredTraits = this.declaredTraits;
            if (declaredTraits == null)
                declaredTraits = this.declaredTraits = new List<object>();
            declaredTraits.Add(trait);
        }

        protected override void CollectDeclaredTraits()
        {
            var declaredTraits = this.declaredTraits;
            if (declaredTraits == null)
                return;
            foreach (var trait in declaredTraits)
                CollectDeclaredTrait(trait);
            this.declaredTraits = null;
        }

        protected override void CollectInheritedTraits()
        {
            var overrides = this.overrides;
            if (overrides == null)
                return;
            foreach (var property in overrides)
                CollectInheritedTraits(property);
            this.overrides = null;
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
