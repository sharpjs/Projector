namespace Projector.Specs
{
    using Projector.ObjectModel;

    public interface ITypeRestriction
    {
        bool AppliesTo(ProjectionType type);
    }
}
