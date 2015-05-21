namespace Projector.ObjectModel
{
    using System;

    public struct PropertyTraitApplicator
    {
        private readonly ProjectionProperty property;

        internal PropertyTraitApplicator(ProjectionProperty property)
        {
            this.property = property;
        }

        public ProjectionProperty Property
        {
            get { return property; }
        }

        public void Apply(object trait)
        {
            if (trait == null)
                throw Error.ArgumentNull("trait");

            property.ApplyTrait(trait, false);
        }

        public void Apply(Type type)
        {
            property.ApplyTrait(Activator.CreateInstance(ConstructType(type)), false);
        }

        public void Apply(Type type, params object[] args)
        {
            property.ApplyTrait(Activator.CreateInstance(ConstructType(type), args), false);
        }

        public TypeTraitApplicator OnContainingType()
        {
            return new TypeTraitApplicator(property.DeclaringType); // TODO: ContainingType
        }

        private Type ConstructType(Type type)
        {
            if (type == null)
                throw Error.ArgumentNull("type");

            if (!type.IsGenericTypeDefinition)
                return type;

            if (type.GetGenericArguments().Length == 1)
                return type.MakeGenericType(property.PropertyType.UnderlyingType);

            throw Error.TodoError();
        }
    }
}
