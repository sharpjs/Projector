namespace Projector.ObjectModel
{
    public interface ITraitBuilder
    {
        void ApplyTraits(ProjectionMetaObject target, TraitApplicator applicator);
    }
}
