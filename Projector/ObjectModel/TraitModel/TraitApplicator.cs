namespace Projector.ObjectModel
{
    // TODO: Is this type useful any more?
    public struct TraitApplicator
    {
        private readonly ProjectionMetaObject target;

        internal TraitApplicator(ProjectionMetaObject target)
        {
            this.target = target;
        }

        public void Apply(object trait, ProjectionMetaObject target)
        {
            if (target == this.target)
                target.Apply(trait);

            throw Error.TodoError();
        }

        //public void ApplyToDeclaringType(object trait, ProjectionProperty property)
        //{
        //    Apply(trait, property.DeclaringType);
        //}

        //public void ApplyToDeclaredProperties(object trait, ProjectionStructureType projectionType)
        //{
        //    foreach (var property in projectionType.Properties)
        //        if (property.DeclaringType == projectionType)
        //            Apply(trait, property);
        //}
    }
}
