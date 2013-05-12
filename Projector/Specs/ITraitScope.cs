namespace Projector.Specs
{
    using System;

    public interface ITraitScope
    {
        ITraitScope Apply(object trait);
        ITraitScope Apply(Func<object> factory);
    }
}
