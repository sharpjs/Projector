namespace Projector.Specs
{
    public interface IPropertyCut : IPropertyScope
    {
        IPropertyCut Matching(IPropertyRestriction restriction);
    }
}
