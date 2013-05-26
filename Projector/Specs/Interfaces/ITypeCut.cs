namespace Projector.Specs
{
    public interface ITypeCut : ITypeBlock
    {
        ITypeCut Matching(ITypeRestriction restriction);
    }
}
