namespace Projector.ObjectModel
{
    public interface ITraitBuilder
    {
        void ApplyTypeTraits    (TypeTraitApplicator     target);
        void ApplyPropertyTraits(PropertyTraitApplicator target);
    }
}
