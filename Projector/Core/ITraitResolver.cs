namespace Projector
{
    using Projector.ObjectModel;

    public interface ITraitResolver
    {
        // TODO: Very similar to ITraitBuilder. Any chance to unify?

        void Resolve(ProjectionType target, TraitApplicator applicator);
    }
}
