namespace Projector.Specs
{
    public interface ISharedScope : Projector.Utility.IFluent
    {
        ITypeCut Types { get; }

        IPropertyCut Properties { get; }

        ITypeBlock<T> Type<T>();
    }
}
