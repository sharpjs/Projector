namespace Projector.Specs
{
    using System;

    public interface ITraitScope : Projector.Utility.IFluent
    {
        ITraitScope Apply(object trait);
        ITraitScope Apply(Func<object> factory);
    }
}
