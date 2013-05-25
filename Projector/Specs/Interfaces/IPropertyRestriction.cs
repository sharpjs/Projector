namespace Projector.Specs
{
    using Projector.ObjectModel;

    public interface IPropertyRestriction
    {
        bool AppliesTo(ProjectionProperty property);
    }
}
