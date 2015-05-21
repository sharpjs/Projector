namespace Projector.ObjectModel
{
    using System;
    using System.Reflection.Emit;

    public interface IMixin
    {
        void     ImplementType(TypeBuilder implementationType);
        void PostImplementType(Type        implementationType);
    }
}
