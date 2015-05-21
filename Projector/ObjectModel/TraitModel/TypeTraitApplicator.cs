namespace Projector.ObjectModel
{
    using System;

    public struct TypeTraitApplicator
    {
        private readonly ProjectionType type;

        internal TypeTraitApplicator(ProjectionType type)
        {
            this.type = type;
        }

        public ProjectionType Type
        {
            get { return type; }
        }

        public void Apply(object trait)
        {
            if (trait == null)
                throw Error.ArgumentNull("trait");

            type.ApplyTrait(trait, false);
        }

        public PropertyTraitApplicator OnProperty(ProjectionProperty property)
        {
            if (property == null)
                throw Error.ArgumentNull("property");

            if (type.Properties[property] == property) // TODO: use ContainingType
                throw Error.ArgumentOutOfRange("property");

            return new PropertyTraitApplicator(property);
        }

        public PropertyTraitApplicator OnProperty(string name)
        {
            return new PropertyTraitApplicator(type.Properties[name]);
        }

        public PropertyTraitApplicator OnProperty(string name, ProjectionType declaringType)
        {
            return new PropertyTraitApplicator(type.Properties[name, declaringType]);
        }
    }
}
