namespace Projector.ObjectModel
{
    public interface IConverter<TOuter, TInner>
    {
        TOuter ToOuter(TInner inner);
        TInner ToInner(TOuter outer);
    }
}
